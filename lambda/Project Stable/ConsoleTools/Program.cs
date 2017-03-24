using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ProjectStableLibrary;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace ConsoleTools {
	public class Program {
		static void Main(string[] args) {
			var conStr = new MySqlConnectionStringBuilder();
			using(StreamReader file = File.OpenText("config.json")) {
				using(JsonTextReader r = new JsonTextReader(file)) {
					JObject conf = (JObject)JToken.ReadFrom(r);
					conStr.Server = (string)conf["server"];
					conStr.Port = uint.Parse((string)conf["port"]);
					conStr.UserID = (string)conf["user"];
					conStr.Password = (string)conf["password"];
					conStr.Database = (string)conf["database"];
				}
			}

			bool clean = true;

			using(StableContext ctx = StableContextFactory.Build(conStr.ToString())) {
				//3 2 4 1
				var viewers = ctx.Viewers;
				Console.WriteLine($"Fetched {viewers.Count} viewers");

				var schedule = ctx.Schedule;

				var capacityCheck = new Dictionary<Schedule, uint>();
				foreach(Schedule s in schedule) {
					capacityCheck.Add(s, 0);
				}
				Console.WriteLine($"Fetched {schedule.Count} schedules");

				var blocks = ctx.Blocks;
				Console.WriteLine($"Fetched {blocks.Count} blocks");

				var preferences = ctx.preferences.ToList();
				Console.WriteLine($"Fetched {preferences.Count} preferences");

				var presentations = ctx.Presentations;

				int expected_count = blocks.Count;
				const int retry_count = 5;

				var toAddToDB = new List<Registration>();

				uint[] grade_pri = new uint[] {3, 2, 4, 1};
				uint high_max = 27;
				uint low_max = 27;

				DateTime start = DateTime.Now;
				foreach(uint g in grade_pri) {
					// if(g != 3)
					// 	continue;
					var viewers_to_proc = new List<uint>();

					viewers_to_proc.AddRange(from thus in viewers where thus.Value.grade_id == g select thus.Value.viewer_id);

					viewers_to_proc.Randomize();

					foreach(uint v in viewers_to_proc) {
						var bleh = new List<Tuple<uint, uint>>();
						var bleh_v = new List<uint>();
						var v_pref = (from thus in preferences where thus.viewer_id == v orderby thus.order select thus.presentation_id).ToList();
						
						bool randomize = v_pref.Count < presentations.Count;
						if(!randomize && (g == 1 || g == 4)) {
							v_pref[v_pref.IndexOf(47)] = v_pref[0];
							v_pref[0] = 47;
						}
						
						
						var temp_s = new Schedule(){ date = 20170324 };
						var blocks_r = blocks.Values.ToList();

						int error_count = 0;
						do {

							blocks_r.Randomize();
							foreach(Block b in blocks_r) {
								temp_s.block_id = b.block_id;

								if(randomize) {
									var r_p = from thus in capacityCheck orderby thus.Value select thus.Key.presentation_id;

									foreach(uint p in r_p) {
										if(bleh_v.Contains(p))
											continue;
										temp_s.presentation_id = p;
										if(!capacityCheck.ContainsKey(temp_s))
											continue;
										if(capacityCheck[temp_s] >= (p != 47 ? high_max : low_max))
											continue;
										capacityCheck[temp_s]++;
										bleh.Add(new Tuple<uint, uint>(b.block_id, p));
										bleh_v.Add(p);
										break;
									}
									continue;
								}

								foreach(uint p in v_pref) {
									if(bleh_v.Contains(p))
										continue;
									temp_s.presentation_id = p;
									if(!capacityCheck.ContainsKey(temp_s))
										continue;
									if(capacityCheck[temp_s] >= (p != 47 ? high_max : low_max))
										continue;
									capacityCheck[temp_s]++;
									bleh.Add(new Tuple<uint, uint>(b.block_id, p));
									bleh_v.Add(p);
									break;
								}

							}
							if(bleh_v.Count != expected_count) {
								//Console.WriteLine("ERROR " + string.Join(", ", bleh_v));

								//undo
								foreach(var toRemove in bleh) {
									temp_s.block_id = toRemove.Item1;
									temp_s.presentation_id = toRemove.Item2;
									capacityCheck[temp_s]--;
								}
								bleh.Clear();
								bleh_v.Clear();
								error_count++;
								if(error_count > retry_count) {
									clean = false;
									break;
								}

							} else {
								foreach(var toAdd in bleh) {
									toAddToDB.Add(new Registration() {
										date = temp_s.date,
										block_id = toAdd.Item1,
										presentation_id = toAdd.Item2,
										viewer_id = v
									});
								}
							}
						} while(bleh_v.Count != expected_count);
						
						//var rng = randomize ? "RNG" : "";
						//Console.WriteLine($"Student: {v} {rng} {string.Join(", ", bleh)}");
					}

					
				}
				DateTime end = DateTime.Now;
				Console.WriteLine($"Took {(end - start).TotalMilliseconds} ms to sort");

				start = DateTime.Now;

				if(clean) {
					using(var tx = ctx.Database.BeginTransaction()) {
						try {
							ctx.Database.ExecuteSqlCommand("DELETE FROM `registrations`;");
							tx.Commit();
						} catch(Exception e) {
							tx.Rollback();
							Console.WriteLine(e);
						}
					}
					using(var tx = ctx.Database.BeginTransaction()) {
						try {
							ctx.registrations.AddRange(toAddToDB);
							ctx.SaveChanges();
							tx.Commit();
						} catch(Exception e) {
							tx.Rollback();
							Console.WriteLine(e);
						}
					}
				}

				end = DateTime.Now;
				Console.WriteLine($"Took {(end - start).TotalMilliseconds} ms to add to db!");

				foreach(var e in capacityCheck) {
					Console.WriteLine(e.Key.block_id + " " + e.Key.presentation_id + " " + e.Value);
				}
				Console.WriteLine($"{toAddToDB.Count} entries to add to DB!");
			}
			Console.WriteLine("Clean: " + clean.ToString());
			Console.WriteLine("Press enter to quit");
			Console.ReadLine();
		}
		
	}
	static class Rng {
		private static Random rng = new Random();  
		public static void Randomize<T>(this IList<T> list) {  
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = rng.Next(n + 1);  
				T value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  
		}
	}
}