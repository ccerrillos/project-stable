using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization;
using Amazon.Lambda.APIGatewayEvents;
using ProjectStableLibrary;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace StableAPIHandler {
	public class Function {

		/// <summary>
		/// A simple function that takes a string and does a ToUpper
		/// </summary>
		/// <param name="input"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		///
		ILambdaLogger Logger;
		public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context) {
			Logger = context.Logger;
			object resultObject = new object();
			int resultCode = 405;


			//Pre check the request path to save time

			switch(apigProxyEvent.Path.ToLower()) {
				case "/":
					return new StableAPIResponse {
						Body = "What are you doing here?",
						StatusCode = HttpStatusCode.OK
					};

				case "/dates":
				case "/dates/":
				case "/blocks":
				case "/blocks/":
				case "/grades":
				case "/grades/":
				case "/houses":
				case "/houses/":
				case "/locations":
				case "/locations/":
				case "/presentations":
				case "/presentations/":
				case "/viewers":
				case "/viewers/":
				case "/schedule":
				case "/schedule/":
				case "/print":
				case "/print/":
					break;

				case "/signup":
				case "/signup/":
					break;

				case "/signup/finish":
				case "/signup/finish/":
					break;

				case "/preferences":
				case "/preferences/":
					break;

				default:
					return new StableAPIResponse {
						Body = "{}",
						StatusCode = HttpStatusCode.NotFound
					};
			}

			StableAPIResponse response = new StableAPIResponse {
				Body = "{}",
				StatusCode = HttpStatusCode.NotFound
			};

			string conStr = new MySqlConnectionStringBuilder() {
				Server = Environment.GetEnvironmentVariable("DB_ADDRESS"),
				Port = uint.Parse(Environment.GetEnvironmentVariable("DB_PORT")),
				UserID = Environment.GetEnvironmentVariable("DB_USER"),
				Password = Environment.GetEnvironmentVariable("DB_PASSWORD"),
				Database = Environment.GetEnvironmentVariable("DB_NAME")
			}.ToString();
			using(StableContext ctx = StableContextFactory.Build(conStr)) {
				switch(apigProxyEvent.HttpMethod) {
					case "GET":
						#region GETs
						switch(apigProxyEvent.Path.ToLower()) {
							case "/":
								resultObject = "What are you doing here?";
								resultCode = 200;
								break;

							case "/dates":
							case "/dates/":
								response = new StableAPIResponse() {
									Body = JsonConvert.SerializeObject(ctx.Dates),
									StatusCode = HttpStatusCode.OK
								};
								break;

							case "/blocks":
							case "/blocks/":
								response = new StableAPIResponse() {
									Body = JsonConvert.SerializeObject(ctx.Blocks),
									StatusCode = HttpStatusCode.OK
								};
								break;

							case "/grades":
							case "/grades/":
								response = new StableAPIResponse() {
									Body = JsonConvert.SerializeObject(ctx.Grades),
									StatusCode = HttpStatusCode.OK
								};
								break;

							case "/houses":
							case "/houses/":
								response = new StableAPIResponse() {
									Body = JsonConvert.SerializeObject(ctx.Houses),
									StatusCode = HttpStatusCode.OK
								};
								break;

							case "/locations":
							case "/locations/":
								response = new StableAPIResponse() {
									Body = JsonConvert.SerializeObject(ctx.Locations),
									StatusCode = HttpStatusCode.OK
								};
								break;

							case "/presentations":
							case "/presentations/":
								response = new StableAPIResponse() {
									Body = JsonConvert.SerializeObject(ctx.Presentations),
									StatusCode = HttpStatusCode.OK
								};
								break;

							case "/viewers":
							case "/viewers/":
								if(apigProxyEvent.QueryStringParameters != null)
								if(apigProxyEvent.QueryStringParameters.Count( thus => thus.Key == "date" || thus.Key == "block_id" || thus.Key == "presentation_id") == 3){
									try {
										uint date = uint.Parse(apigProxyEvent.QueryStringParameters["date"]);
										uint block_id = uint.Parse(apigProxyEvent.QueryStringParameters["block_id"]);
										uint presentation_id = uint.Parse(apigProxyEvent.QueryStringParameters["presentation_id"]);

										var regs = ctx.registrations.Where(thus => thus.date == date && thus.block_id == block_id && thus.presentation_id == presentation_id).ToList();
										var viewers = ctx.viewers.ToList();
										var result = new List<SanitizedViewer>();
										foreach(var r in regs) {
											result.Add(viewers.Find(thus => thus.viewer_id == r.viewer_id).Sanitize());
										}
										response = new StableAPIResponse() {
											Body = JsonConvert.SerializeObject(result),
											StatusCode = HttpStatusCode.OK
										};
										break;
									} catch (Exception e) {
										Logger.LogLine(e.ToString());
										response = new StableAPIResponse() {
											Body = JsonConvert.SerializeObject(new Result(e)),
											StatusCode = HttpStatusCode.BadRequest
										};
									}
									break;
								}
								response = new StableAPIResponse() {
									Body = JsonConvert.SerializeObject(ctx.Viewers),
									StatusCode = HttpStatusCode.OK
								};
								break;

							case "/preferences":
							case "/preferences/":
								try {
									uint viewer_id = uint.Parse(apigProxyEvent.QueryStringParameters["viewer_id"]);
									response = new StableAPIResponse() {
										Body = JsonConvert.SerializeObject(ctx.GetPreferences(viewer_id)),
										StatusCode = HttpStatusCode.OK
									};
								} catch (Exception e) {
									response = new StableAPIResponse() {
										Body = JsonConvert.SerializeObject(new Result(e)),
										StatusCode = HttpStatusCode.BadRequest
									};
								}
								break;
							
							case "/schedule":
							case "/schedule/":
								if(apigProxyEvent.QueryStringParameters != null)
									if(apigProxyEvent.QueryStringParameters.ContainsKey("viewer_id")) {
										try {
											uint viewer_id = uint.Parse(apigProxyEvent.QueryStringParameters["viewer_id"]);

											var regs = ctx.registrations.Where(thus => thus.viewer_id == viewer_id).ToList();
											response = new StableAPIResponse() {
												Body = JsonConvert.SerializeObject(regs),
												StatusCode = HttpStatusCode.OK
											};
											break;
										} catch (Exception e) {
											Logger.LogLine(e.ToString());
											response = new StableAPIResponse() {
												Body = JsonConvert.SerializeObject(new Result(e)),
												StatusCode = HttpStatusCode.BadRequest
											};
										}
										break;
									}
								response = new StableAPIResponse() {
									Body = JsonConvert.SerializeObject(ctx.Schedule),
									StatusCode = HttpStatusCode.OK
								};
								break;
							case "/print":
							case "/print/":
								response = HandlePrint(apigProxyEvent, ctx);
								break;

							default:
								break;
						}
						#endregion
						break;
					case "POST":
						#region POSTs
						switch(apigProxyEvent.Path.ToLower()) {
							case "/dates":
							case "/dates/":
								response = HandlePOST<Date>(apigProxyEvent, ctx);
								break;
							case "/blocks":
							case "/blocks/":
								response = HandlePOST<Block>(apigProxyEvent, ctx);
								break;
							case "/grades":
							case "/grades/":
								response = HandlePOST<Grade>(apigProxyEvent, ctx);
								break;
							case "/houses":
							case "/houses/":
								response = HandlePOST<House>(apigProxyEvent, ctx);
								break;
							case "/locations":
							case "/locations/":
								response = HandlePOST<Location>(apigProxyEvent, ctx);
								break;
							case "/presentations":
							case "/presentations/":
								response = HandlePOST<Presentation>(apigProxyEvent, ctx);
								break;
							case "/viewers":
							case "/viewers/":
								//response = HandlePOST<Viewer>(apigProxyEvent, ctx);
								break;

							case "/signup":
							case "/signup/":
								response = startSignup(apigProxyEvent, ctx);
								break;

							case "/signup/finish":
							case "/signup/finish/":
								response = finishSignup(apigProxyEvent, ctx, context);
								break;
						}
						#endregion
						break;
					case "PUT":
						#region PUTs
						switch(apigProxyEvent.Path.ToLower()) {
							case "/dates":
							case "/dates/":
								response = HandlePUT<Date>(apigProxyEvent, ctx);
								break;
							case "/blocks":
							case "/blocks/":
								response = HandlePUT<Block>(apigProxyEvent, ctx);
								break;
							case "/grades":
							case "/grades/":
								response = HandlePUT<Grade>(apigProxyEvent, ctx);
								break;
							case "/houses":
							case "/houses/":
								response = HandlePUT<House>(apigProxyEvent, ctx);
								break;
							case "/locations":
							case "/locations/":
								response = HandlePUT<Location>(apigProxyEvent, ctx);
								break;
							case "/presentations":
							case "/presentations/":
								response = HandlePUT<Presentation>(apigProxyEvent, ctx);
								break;
							case "/viewers":
							case "/viewers/":
								response = HandlePUT<Viewer>(apigProxyEvent, ctx);
								break;
						}
						break;
					#endregion
					case "DELETE":
						#region DELETEs
						switch(apigProxyEvent.Path.ToLower()) {
							case "/dates":
							case "/dates/":
								response = HandleDELETE<Date>(apigProxyEvent, ctx);
								break;
							case "/blocks":
							case "/blocks/":
								response = HandleDELETE<Block>(apigProxyEvent, ctx);
								break;
							case "/grades":
							case "/grades/":
								response = HandleDELETE<Grade>(apigProxyEvent, ctx);
								break;
							case "/houses":
							case "/houses/":
								response = HandleDELETE<House>(apigProxyEvent, ctx);
								break;
							case "/locations":
							case "/locations/":
								response = HandleDELETE<Location>(apigProxyEvent, ctx);
								break;
							case "/presentations":
							case "/presentations/":
								response = HandleDELETE<Presentation>(apigProxyEvent, ctx);
								break;
							case "/viewers":
							case "/viewers/":
								response = HandleDELETE<Viewer>(apigProxyEvent, ctx);
								break;
							default:
								break;
						}
						#endregion
						break;
					default:
						break;
				}
			}
			//Logger.LogLine($"RESPONSE CODE: {((HttpStatusCode)response.StatusCode).ToString()}{Environment.NewLine}{response.Body}");

			return response;
		}

		//You gotta love generic typing!! :D

		private StableAPIResponse HandlePOST<E> (APIGatewayProxyRequest request, StableContext ctx) where E : class {
			try {
				string adminCode = Environment.GetEnvironmentVariable("admin_code");
				if(adminCode == null || adminCode == "")
					throw new InvalidOperationException("admin_code not set on server");

				if(!request.Headers.ContainsKey("admin_code"))
					throw new ArgumentException("admin_code is missing");

				if(request.Headers["admin_code"] != adminCode)
					throw new UnauthorizedAccessException("Invalid admin_code");

				E obj = JsonConvert.DeserializeObject<E>(request.Body);

				using(var tx = ctx.Database.BeginTransaction()) {
					try {
						ctx.Add(obj);
						int status = ctx.SaveChanges();
						tx.Commit();
						return new StableAPIResponse() {
							Body = JsonConvert.SerializeObject((status == 1)),
							StatusCode = HttpStatusCode.OK
						};
					} catch(Exception e) {
						tx.Rollback();
						Logger.LogLine(e.ToString());
						return new StableAPIResponse() {
							Body = JsonConvert.SerializeObject(new Result(e)),
							StatusCode = HttpStatusCode.InternalServerError
						};
					}
				}

			} catch(Exception e) {
				Logger.LogLine(e.ToString());
				return new StableAPIResponse() {
					Body = JsonConvert.SerializeObject(new Result(e)),
					StatusCode = HttpStatusCode.BadRequest
				};
			}
		}
		private StableAPIResponse HandlePUT<E>(APIGatewayProxyRequest request, StableContext ctx) where E : class {
			try {
				string adminCode = Environment.GetEnvironmentVariable("admin_code");
				if(adminCode == null || adminCode == "")
					throw new InvalidOperationException("admin_code not set on server");

				if(!request.Headers.ContainsKey("admin_code"))
					throw new ArgumentException("admin_code is missing");

				if(request.Headers["admin_code"] != adminCode)
					throw new UnauthorizedAccessException("Invalid admin_code");

				E obj = JsonConvert.DeserializeObject<E>(request.Body);

				using(var tx = ctx.Database.BeginTransaction()) {
					try {
						var existing = ctx.Attach<E>(obj);
						ctx.Entry(existing).State = EntityState.Modified;
						int status = ctx.SaveChanges();
						tx.Commit();
						return new StableAPIResponse() {
							Body = JsonConvert.SerializeObject((status == 1)),
							StatusCode = HttpStatusCode.OK
						};
					} catch(Exception e) {
						tx.Rollback();
						Logger.LogLine(e.ToString());
						return new StableAPIResponse() {
							Body = JsonConvert.SerializeObject(new Result(e)),
							StatusCode = HttpStatusCode.InternalServerError
						};
					}
				}

			} catch(Exception e) {
				Logger.LogLine(e.ToString());
				return new StableAPIResponse() {
					Body = JsonConvert.SerializeObject(new Result(e)),
					StatusCode = HttpStatusCode.BadRequest
				};
			}
		}
		private StableAPIResponse HandleDELETE<E>(APIGatewayProxyRequest request, StableContext ctx) where E : class {
			try {
				string adminCode = Environment.GetEnvironmentVariable("admin_code");
				if(adminCode == null || adminCode == "")
					throw new InvalidOperationException("admin_code not set on server");

				if(!request.Headers.ContainsKey("admin_code"))
					throw new ArgumentException("admin_code is missing");

				if(request.Headers["admin_code"] != adminCode)
					throw new UnauthorizedAccessException("Invalid admin_code");

				E obj = JsonConvert.DeserializeObject<E>(request.Body);
				/*
				 * Gotta wrap DB ops in a transaction
				 * otherwise, if they die in a try catch
				 * it could leave an uncommitted tx on the db
				 * causing problems with future requests
				using(var tx = ctx.Database.BeginTransaction()) {
					
				}
				*/
				using(var tx = ctx.Database.BeginTransaction()) {
					try {
						//Logger.LogLine(obj.GetType().ToString());
						ctx.Remove(obj);
						//ctx.Attach(obj);
						//ctx.Remove(obj);
						//ctx.dates.Remove(ctx.dates.Single(thus => thus.date == date));
						int status = ctx.SaveChanges();
						tx.Commit();
						return new StableAPIResponse() {
								Body = JsonConvert.SerializeObject((status == 1)),
								StatusCode = HttpStatusCode.OK
							};
					} catch(Exception e) {
						tx.Rollback();
						Logger.LogLine(e.ToString());
						return new StableAPIResponse() {
							Body = JsonConvert.SerializeObject(new Result(e)),
							StatusCode = HttpStatusCode.InternalServerError
						};
					}
				}

			} catch(Exception e) {
				Logger.LogLine(e.ToString());
				return new StableAPIResponse() {
					Body = JsonConvert.SerializeObject(new Result(e)),
					StatusCode = HttpStatusCode.BadRequest
				};
			}
		}
		private StableAPIResponse startSignup(APIGatewayProxyRequest request, StableContext ctx) {
			try {
				SignupRequest sr = JsonConvert.DeserializeObject<SignupRequest>(request.Body);
				try {
					// Create viewer entry first, so if they don't submit properly
					// we'll have their info and can randomly place them.
					Viewer v = new Viewer() {
						first_name = sr.first_name,
						last_name = sr.last_name,
						grade_id = sr.grade,
						house_id = sr.house,
						viewer_key = Guid.NewGuid().ToString().Substring(0, 16)
					};

					using(var tx = ctx.Database.BeginTransaction()) {
						try {
							ctx.viewers.Add(v);
							ctx.SaveChanges();
							tx.Commit();

							return new StableAPIResponse() {
								Body = JsonConvert.SerializeObject(new SignupResponse(v) {
									status = true
								}),
								StatusCode = HttpStatusCode.OK
							};

						} catch(Exception e) {
							tx.Rollback();
							return new StableAPIResponse() {
								Body = JsonConvert.SerializeObject(new Result(e)),
								StatusCode = HttpStatusCode.InternalServerError
							};
						}
					}
				} catch (Exception e) {
					return new StableAPIResponse() {
						Body = JsonConvert.SerializeObject(new Result(e)),
						StatusCode = HttpStatusCode.InternalServerError
					};
				}
			} catch(Exception e) {
				return new StableAPIResponse() {
					Body = JsonConvert.SerializeObject(new Result(e)),
					StatusCode = HttpStatusCode.BadRequest
				};
			}
		}
		private StableAPIResponse finishSignup(APIGatewayProxyRequest apigProxyEvent, StableContext ctx, ILambdaContext context) {
			try {
				var req = JsonConvert.DeserializeObject<FinishSignupRequest>(apigProxyEvent.Body);
				req.status = true;
				try {
					List<Preference> toAdd = new List<Preference>();
					for(int x = 0; x < req.data.Count; x++) {
						toAdd.Add(new Preference() {
							viewer_id = req.viewer_id,
							order = (uint)(x + 1),
							presentation_id = req.data[x]
						});
					}
					using(var tx = ctx.Database.BeginTransaction()) {
						try {
							foreach(Preference p in toAdd) {
								ctx.preferences.Add(p);
							}
							ctx.SaveChanges();
							tx.Commit();
						} catch (DbUpdateException e) {
							tx.Rollback();
							if(e.InnerException != null) {
								if(e.InnerException.GetType() == typeof(MySqlException)) {
									var me = e.InnerException as MySqlException;
									return new StableAPIResponse() {
										Body = JsonConvert.SerializeObject(new SignupErrorResponse(me.Number)),
										StatusCode = HttpStatusCode.OK
									};
								}
							}
							return new StableAPIResponse() {
								Body = JsonConvert.SerializeObject(e),
								StatusCode = HttpStatusCode.InternalServerError
							};
						} catch(Exception e) {
							tx.Rollback();
							var expt = e;
							while(expt != null) {
								context.Logger.LogLine(expt.Message);
								expt = expt.InnerException;
							}
							return new StableAPIResponse() {
								Body = JsonConvert.SerializeObject(new Result(e)),
								StatusCode = HttpStatusCode.InternalServerError
							};
							
						}
					}

				} catch(Exception e) {
					return new StableAPIResponse() {
						Body = JsonConvert.SerializeObject(new Result(e)),
						StatusCode = HttpStatusCode.InternalServerError
					};
					
				}
				return new StableAPIResponse() {
					StatusCode = HttpStatusCode.OK,
					Body = JsonConvert.SerializeObject(req)
				};
			} catch(Exception e) {
				return new StableAPIResponse() {
					Body = JsonConvert.SerializeObject(new Result(e)),
					StatusCode = HttpStatusCode.BadRequest
				};
			}
		}
		private StableAPIResponse HandlePrint(APIGatewayProxyRequest request, StableContext ctx) {

			uint pres_id;
			try {
				try {
				pres_id = uint.Parse(request.QueryStringParameters["presentation_id"]);
				} catch(Exception e) {
					return new StableAPIResponse() {
						Body = JsonConvert.SerializeObject(new Result(e)),
						StatusCode = HttpStatusCode.BadRequest
					};
				}
				var presentation = ctx.presentations.First(thus => thus.presentation_id == pres_id);
				var location = ctx.locations.First(thus => thus.location_id == presentation.location_id);
				var blocks = ctx.Blocks;
				var grades = ctx.Grades;
				var houses = ctx.Houses;

				var schedules = ctx.schedule.Where(thus => thus.presentation_id == pres_id).ToList();
				var viewers = new Dictionary<Schedule, List<Viewer>>();

				foreach(Schedule s in schedules) {
					viewers.Add(s, new List<Viewer>());
				}

				var temp = ctx.registrations.Where(thus => thus.presentation_id == pres_id).ToList();

				var viewers_in_pres = from thus in temp select thus.viewer_id;
				
				var viewers_with_data = ctx.viewers.Where(thus => viewers_in_pres.Contains(thus.viewer_id)).ToList();

				foreach(Registration r in temp) {
					viewers[r.Schedule].Add(viewers_with_data.First(thus => thus.viewer_id == r.viewer_id));
				}


				PrintOutput print = new PrintOutput() {
					presentationData = presentation,
					locationData = location,
					blocks = blocks,
					schedule = schedules,
					grades = grades,
					houses = houses,
					viewers = viewers

				};
				var res = new StableAPIResponse() {
					Body = print.ToString(),
					StatusCode = HttpStatusCode.OK,
					
				};
				res.Headers.Add("Content-Type", "text/html; charset=utf-8");
				return res;
			} catch(Exception e) {
				throw;
			}


			return new StableAPIResponse() {
				Body = "",
				StatusCode = HttpStatusCode.NotImplemented
			};
		}

	}
	public class StableAPIResponse : APIGatewayProxyResponse {
		public StableAPIResponse() {
			Headers = new Dictionary<string, string>() {
				{ "access-control-allow-origin", Environment.GetEnvironmentVariable("SITE_DOMAIN") }
			};
		}
		new public HttpStatusCode StatusCode {
			get {
				return (HttpStatusCode)StatusCode;
			}
			set {
				base.StatusCode = (int)value;
			}
		}
	}
}
