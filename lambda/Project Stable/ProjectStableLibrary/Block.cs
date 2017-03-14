using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProjectStableLibrary {
	[Table("blocks")]
	public class Block {
		[Key]
		public uint block_id {
			get;
			set;
		}
		[MaxLength(255)]
		public string block_name {
			get;
			set;
		}
	}
}
