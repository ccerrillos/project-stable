using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProjectStableLibrary {
	[Table("grades")]
	public class Grade {
		[Key]
		public uint grade_id {
			get;
			set;
		}
		[MaxLength(255)]
		public string grade_name {
			get;
			set;
		}
		public uint default_amount {
			get;
			set;
		}
	}
}
