using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using Amazon.Lambda.APIGatewayEvents;
using System.Net;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectStableLibrary {
	[Table("houses")]
	public class House {
		[Key]
		public int house_id {
			get;
			set;
		}
		public string house_name {
			get;
			set;
		}

		public override string ToString() {
			return $"house_id: {house_id} house_name: {house_name}";
		}
	}
}
