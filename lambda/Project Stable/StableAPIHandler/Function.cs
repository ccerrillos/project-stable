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
						StatusCode = (int)HttpStatusCode.OK
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
					break;

				default:
					return new StableAPIResponse {
						Body = "{}",
						StatusCode = (int)HttpStatusCode.NotFound
					};
			}

			StableAPIResponse response = new StableAPIResponse {
				Body = "{}",
				StatusCode = (int)HttpStatusCode.NotFound
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
									StatusCode = (int)HttpStatusCode.OK
								};
								break;

							case "/blocks":
							case "/blocks/":
								response = new StableAPIResponse() {
									Body = JsonConvert.SerializeObject(ctx.Blocks),
									StatusCode = (int)HttpStatusCode.OK
								};
								break;

							case "/grades":
							case "/grades/":
								response = new StableAPIResponse() {
									Body = JsonConvert.SerializeObject(ctx.Grades),
									StatusCode = (int)HttpStatusCode.OK
								};
								break;

							case "/houses":
							case "/houses/":
								response = new StableAPIResponse() {
									Body = JsonConvert.SerializeObject(ctx.Houses),
									StatusCode = (int)HttpStatusCode.OK
								};
								break;

							case "/locations":
							case "/locations/":
								response = new StableAPIResponse() {
									Body = JsonConvert.SerializeObject(ctx.Locations),
									StatusCode = (int)HttpStatusCode.OK
								};
								break;

							case "/presentations":
							case "/presentations/":
								response = new StableAPIResponse() {
									Body = JsonConvert.SerializeObject(ctx.Presentations),
									StatusCode = (int)HttpStatusCode.OK
								};
								break;

							case "/viewers":
							case "/viewers/":
								response = new StableAPIResponse() {
									Body = JsonConvert.SerializeObject(ctx.Viewers),
									StatusCode = (int)HttpStatusCode.OK
								};
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
						}
						#endregion
						break;
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

			return response;
		}

		//You gotta love generic typing!! :D

		private StableAPIResponse HandlePOST<E> (APIGatewayProxyRequest request, StableContext ctx) where E : class {
			try {
				E obj = JsonConvert.DeserializeObject<E>(request.Body);

				using(var tx = ctx.Database.BeginTransaction()) {
					try {
						ctx.Add(obj);
						int status = ctx.SaveChanges();
						tx.Commit();
						return new StableAPIResponse() {
							Body = JsonConvert.SerializeObject((status == 1)),
							StatusCode = (int)HttpStatusCode.OK
						};
					} catch(Exception e) {
						tx.Rollback();
						Logger.LogLine(e.ToString());
						return new StableAPIResponse() {
							Body = JsonConvert.SerializeObject(new Result(e)),
							StatusCode = (int)HttpStatusCode.InternalServerError
						};
					}
				}

			} catch(Exception e) {
				Logger.LogLine(e.ToString());
				return new StableAPIResponse() {
					Body = JsonConvert.SerializeObject(new Result(e)),
					StatusCode = (int)HttpStatusCode.BadRequest
				};
			}
		}
		private StableAPIResponse HandleDELETE<E>(APIGatewayProxyRequest request, StableContext ctx) where E : class {
			try {
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
								StatusCode = (int)HttpStatusCode.OK
							};
					} catch(Exception e) {
						tx.Rollback();
						Logger.LogLine(e.ToString());
						return new StableAPIResponse() {
							Body = JsonConvert.SerializeObject(new Result(e)),
							StatusCode = (int)HttpStatusCode.InternalServerError
						};
					}
				}

			} catch(Exception e) {
				Logger.LogLine(e.ToString());
				return new StableAPIResponse() {
					Body = JsonConvert.SerializeObject(new Result(e)),
					StatusCode = (int)HttpStatusCode.BadRequest
				};
			}
		}
	}
	public class StableAPIResponse : APIGatewayProxyResponse {
		public StableAPIResponse() {
			Headers = new Dictionary<string, string>() {
				{ "access-control-allow-origin", Environment.GetEnvironmentVariable("SITE_DOMAIN") }
			};
		}
	}
}
