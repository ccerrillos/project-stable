using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ProjectStableLibrary {
	public class StableContextFactory {
		public static StableContext Build(string conStr) {
			var optionsBuilder = new DbContextOptionsBuilder<StableContext>();
			//optionsBuilder.
			optionsBuilder.UseMySQL(conStr);

			return new StableContext(optionsBuilder.Options);
		}
	}
	public class StableContext : DbContext {
		public StableContext(DbContextOptions<StableContext> options) : base(options) {
			//Model.
		}

		public DbSet<Date> dates {
			get;
			set;
		}
		public List<int> Dates {
			get {
				List<int> dates = new List<int>();
				var list = from d in this.dates orderby d.date select d.date;
				foreach(int i in list) {
					dates.Add(i);
				}
				return dates;
			}
		}
		public DbSet<Block> blocks {
			get;
			set;
		}
		public Dictionary<int, Block> Blocks {
			get {
				Dictionary<int, Block> blocks = new Dictionary<int, Block>();
				foreach(Block b in this.blocks) {
					blocks.Add(b.block_id, b);
				}

				return blocks;
			}
		}
		public DbSet<Grade> grade_levels {
			get;
			set;
		}
		public Dictionary<int, Grade> Grades {
			get {
				Dictionary<int, Grade> grades = new Dictionary<int, Grade>();
				foreach(Grade g in this.grade_levels) {
					grades.Add(g.grade_id, g);
				}

				return grades;
			}
		}
		public DbSet<House> houses {
			get;
			set;
		}
		public Dictionary<int, House> Houses {
			get {
				Dictionary<int, House> houses = new Dictionary<int, House>();
				foreach(House h in this.houses) {
					houses.Add(h.house_id, h);
				}

				return houses;
			}
		}
		public DbSet<Location> locations {
			get;
			set;
		}
		public Dictionary<int, Location> Locations {
			get {
				Dictionary<int, Location> locations = new Dictionary<int, Location>();
				foreach(Location l in this.locations) {
					locations.Add(l.location_id, l);
				}

				return locations;
			}
		}
		public DbSet<Viewer> viewers {
			get;
			set;
		}
		public Dictionary<int, Viewer> Viewers {
			get {
				Dictionary<int, Viewer> viewers = new Dictionary<int, Viewer>();
				foreach(Viewer v in this.viewers) {
					viewers.Add(v.viewer_id, v);
				}

				return viewers;
			}
		}

	}
	
}
