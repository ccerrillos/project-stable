using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HousesAPI {
	public class House {
		public int house_id;
		public string house_name;
	}
	public class Function {

		[Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
		public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest apigProxyEvent) {
			object resultObject = new object();
			int resultCode = 405;

			switch(apigProxyEvent.HttpMethod) {
				case "GET":
					resultObject = new House() {
						house_id = 1,
						house_name = "North"
					};
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
