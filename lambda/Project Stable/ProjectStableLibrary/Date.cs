using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProjectStableLibrary {
	[Table("dates")]
	public class Date {
		[Key]
		public uint date {
			get;
			set;
		}
		public override string ToString() {
			return $"date: {date}";
		}
	}
}
