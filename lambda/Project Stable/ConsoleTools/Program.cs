using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ProjectStableLibrary;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

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

				uint[] grade_pri = new uint[] {3, 2, 4, 1};
				uint high_max = 26;
				uint low_max = 20;
				foreach(uint g in grade_pri) {
					// if(g != 3)
					// 	continue;
					var viewers_to_proc = new List<uint>();

					viewers_to_proc.AddRange(from thus in viewers where thus.Value.grade_id == g select thus.Value.viewer_id);

					viewers_to_proc.Randomize();

					foreach(uint v in viewers_to_proc) {
						var bleh = new List<Tuple<uint, uint>>();
						var bleh_v = new List<uint>();
						var v_pref = preferences.Where(thus => thus.viewer_id == v).OrderBy(thus => thus.order).ToList();

						bool randomize = v_pref.Count < presentations.Count;
						
						var temp_s = new Schedule(){ date = 20170324 };
						foreach(Block b in blocks.Values) {
							temp_s.block_id = b.block_id;
							
							if(randomize) {
								var r_p = presentations.Keys.ToList();
								r_p.Randomize();

								int index = r_p.IndexOf(47);
								r_p[index] = r_p[0];
								r_p[0] = 47;

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

							foreach(Preference p in v_pref) {
								if(bleh_v.Contains(p.presentation_id))
									continue;
								temp_s.presentation_id = p.presentation_id;
								if(!capacityCheck.ContainsKey(temp_s))
									continue;
								if(capacityCheck[temp_s] >= (p.presentation_id != 47 ? high_max : low_max))
									continue;
								capacityCheck[temp_s]++;
								bleh.Add(new Tuple<uint, uint>(b.block_id, p.presentation_id));
								bleh_v.Add(p.presentation_id);
								break;
							}
							
						}
						if(bleh_v.Count != 5) {
							Console.WriteLine("ERROR " + string.Join(", ", bleh_v));
							clean = false;
						}
						var rng = randomize ? "RNG" : "";
						Console.WriteLine($"Student: {v} {rng} {string.Join(", ", bleh)}");
					}

					
				}
				foreach(var e in capacityCheck) {
					Console.WriteLine(e.Key.block_id + " " + e.Key.presentation_id + " " + e.Value);
				}
			}
			Console.WriteLine("Clean: " + clean.ToString());
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