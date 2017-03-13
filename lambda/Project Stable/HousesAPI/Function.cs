using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HousesAPI {
	public class House {
		public int house_id;
		public string house_name;
		public static Dictionary<int, House> GetHouses() {
			Dictionary<int, House> houses = new Dictionary<int, House>();
			House house;

			MySqlConnectionStringBuilder conStr = new MySqlConnectionStringBuilder();

			conStr.Server = Environment.GetEnvironmentVariable("DB_ADDRESS");
			conStr.Port = uint.Parse(Environment.GetEnvironmentVariable("DB_PORT"));
			conStr.UserID = Environment.GetEnvironmentVariable("DB_USER");
			conStr.Password = Environment.GetEnvironmentVariable("DB_PASSWORD");
			conStr.Database = Environment.GetEnvironmentVariable("DB_NAME");

			conStr.ConnectionTimeout = 20;

			try {

				using(MySqlConnection dbCon = new MySqlConnection(conStr.ToString())) {
					dbCon.Open();

					string query = "SELECT `house_id`, `house_name` FROM `houses`;";

					using(MySqlCommand cmd = new MySqlCommand(query, dbCon)) {

						using(MySqlDataReader r = cmd.ExecuteReader()) {
							while(r.Read()) {
								house = new House() {
									house_id = r.GetInt32(0),
									house_name = r.GetString(1)
								};
								houses.Add(house.house_id, house);
							}
						}
					}

					dbCon.Close();
				}
			} catch(Exception e) {
				LambdaLogger.Log(e.ToString());
				throw;
			}

			return houses;
		}
		public override string ToString() {
			return $"house_id: {house_id} house_name: {house_name}";
		}
	}
	public class Function {
		public static ILambdaContext context;
		[Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
		public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context) {
			Function.context = context;
			object resultObject = new object();
			int resultCode = 405;

			switch(apigProxyEvent.HttpMethod) {
				case "GET":
					resultObject = House.GetHouses();
					resultCode = 200;
					break;
				case "POST":
					resultCode = 200;
					break;
				default:
					resultCode = 405;
					break;

			}
			
			return new APIGatewayProxyResponse {
				Body = JsonConvert.SerializeObject(resultObject),
				StatusCode = resultCode
			};
		}
	}
}
