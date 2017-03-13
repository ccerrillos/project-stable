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
		public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context) {
			object resultObject = new object();
			int resultCode = 405;


			//Pre check the request path to save time

			switch(apigProxyEvent.Path.ToLower()) {
				case "/":
					return new APIGatewayProxyResponse {
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
					return new APIGatewayProxyResponse {
						Body = "{}",
						StatusCode = (int)HttpStatusCode.NotFound
					};
			}

			APIGatewayProxyResponse response = new APIGatewayProxyResponse {
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
								response = new APIGatewayProxyResponse() {
									Body = JsonConvert.SerializeObject(ctx.Dates),
									StatusCode = (int)HttpStatusCode.OK
								};
								break;

							case "/blocks":
							case "/blocks/":
								response = new APIGatewayProxyResponse() {
									Body = JsonConvert.SerializeObject(ctx.Blocks),
									StatusCode = (int)HttpStatusCode.OK
								};
								break;

							case "/grades":
							case "/grades/":
								response = new APIGatewayProxyResponse() {
									Body = JsonConvert.SerializeObject(ctx.Grades),
									StatusCode = (int)HttpStatusCode.OK
								};
								break;

							case "/houses":
							case "/houses/":
								response = new APIGatewayProxyResponse() {
									Body = JsonConvert.SerializeObject(ctx.Houses),
									StatusCode = (int)HttpStatusCode.OK
								};
								break;

							case "/locations":
							case "/locations/":
								response = new APIGatewayProxyResponse() {
									Body = JsonConvert.SerializeObject(ctx.Locations),
									StatusCode = (int)HttpStatusCode.OK
								};
								break;

							case "/presentations":
							case "/presentations/":
								break;

							case "/viewers":
							case "/viewers/":
								response = new APIGatewayProxyResponse() {
									Body = JsonConvert.SerializeObject(ctx.Viewers),
									StatusCode = (int)HttpStatusCode.OK
								};
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
	}
}
