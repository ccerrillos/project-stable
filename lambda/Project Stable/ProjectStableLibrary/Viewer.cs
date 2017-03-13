using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProjectStableLibrary {
	[Table("viewers")]
	public class Viewer {
		[Key]
		public int viewer_id {
			get;
			set;
		}
		[MaxLength(255)]
		public string first_name {
			get;
			set;
		}
		[MaxLength(255)]
		public string last_name {
			get;
			set;
		}
	}
}
