using System;
using System.Collections.Generic;
using Amazon.Lambda.Core;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MySQL.Data.EntityFrameworkCore.Extensions;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace MySQLEntityTest {
	public class ProjectStableContext : DbContext {
		public ProjectStableContext(DbContextOptions<ProjectStableContext> options): base(options) {
			
		}
		public DbSet<Houses> houses {
			get;
			set;
		}

	}
	[Table("houses")]
	public class Houses {
		[Key]
		public int house_id {
			get;
			set;
		}
		[MaxLength(255)]
		public string house_name {
			get;
			set;
		}
		public override string ToString() {
			return $"house_id: {house_id} house_name: {house_name}";
		}
	}
	public class Function {

		/// <summary>
		/// A simple function that takes a string and does a ToUpper
		/// </summary>
		/// <param name="input"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public string FunctionHandler(ILambdaContext context) {

			MySqlConnectionStringBuilder conStr = new MySqlConnectionStringBuilder();
			conStr.Server = Environment.GetEnvironmentVariable("DB_ADDRESS");
			conStr.Port = uint.Parse(Environment.GetEnvironmentVariable("DB_PORT"));
			conStr.UserID = Environment.GetEnvironmentVariable("DB_USER");
			conStr.Password = Environment.GetEnvironmentVariable("DB_PASSWORD");
			conStr.Database = Environment.GetEnvironmentVariable("DB_NAME");

			var options = new DbContextOptionsBuilder<ProjectStableContext>();
			options.UseMySQL(conStr.ToString());

			Dictionary<int, Houses> houses = new Dictionary<int, Houses>();

			using(ProjectStableContext ctx = new ProjectStableContext(options.Options)) {
				
				foreach(Houses h in ctx.houses.ToList()) {
					houses.Add(h.house_id, h);
					context.Logger.LogLine(h.ToString());
				}
			}


			return JsonConvert.SerializeObject(houses);
		}
	}
}
