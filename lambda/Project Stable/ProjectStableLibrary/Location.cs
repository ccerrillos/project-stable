using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProjectStableLibrary {
	[Table("locations")]
	public class Location {
		[Key]
		public int location_id {
			get;
			set;
		}
		[MaxLength(255)]
		public string location_name {
			get;
			set;
		}
	}
}
