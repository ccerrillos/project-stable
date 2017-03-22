using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProjectStableLibrary {
	[Table("schedule")]
	public class Schedule {
		[Key, Column(Order = 1)]
		public uint date {
			get;
			set;
		}
		[Key, Column(Order = 2)]
		public uint block_id {
			get;
			set;
		}
		[Key, Column(Order = 3)]
		public uint presentation_id {
			get;
			set;
		}
	}
}
