using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProjectStableLibrary {
	[Table("presentations")]
	public class Presentation {
		[Key]
		public uint presentation_id {
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
		[MaxLength(255)]
		public string topic {
			get;
			set;
		}
		public uint date {
			get;
			set;
		}
		public uint block_id {
			get;
			set;
		}
		public uint location_id {
			get;
			set;
		}
	}
}
