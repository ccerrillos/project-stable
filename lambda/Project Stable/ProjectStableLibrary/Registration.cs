using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProjectStableLibrary {
	[Table("registrations")]
	public class Registration {
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
		public uint viewer_id {
			get;
			set;
		}
		public uint presentation_id {
			get;
			set;
		}
		public Schedule Schedule {
			get {
				return new Schedule() {
					date = date,
					block_id = block_id,
					presentation_id = presentation_id
				};
			}
		}
	}
}
