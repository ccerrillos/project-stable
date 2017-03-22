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
		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<Preference>()
				.HasKey(c => new {
					c.viewer_id,
					c.order
				});
			modelBuilder.Entity<Schedule>()
				.HasKey(c => new {
					c.date,
					c.block_id,
					c.presentation_id
				});
		}

		public DbSet<Date> dates {
			get;
			set;
		}
		public List<Date> Dates {
			get {
				return dates.ToList();
			}
		}
		public DbSet<Block> blocks {
			get;
			set;
		}
		public Dictionary<uint, Block> Blocks {
			get {
				Dictionary<uint, Block> blocks = new Dictionary<uint, Block>();
				foreach(Block b in this.blocks) {
					blocks.Add(b.block_id, b);
				}

				return blocks;
			}
		}
		public DbSet<Grade> grades {
			get;
			set;
		}
		public Dictionary<uint, Grade> Grades {
			get {
				Dictionary<uint, Grade> grades = new Dictionary<uint, Grade>();
				foreach(Grade g in this.grades) {
					grades.Add(g.grade_id, g);
				}

				return grades;
			}
		}
		public DbSet<House> houses {
			get;
			set;
		}
		public Dictionary<uint, House> Houses {
			get {
				Dictionary<uint, House> houses = new Dictionary<uint, House>();
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
		public Dictionary<uint, Location> Locations {
			get {
				Dictionary<uint, Location> locations = new Dictionary<uint, Location>();
				foreach(Location l in this.locations) {
					locations.Add(l.location_id, l);
				}

				return locations;
			}
		}
		public DbSet<Presentation> presentations {
			get;
			set;
		}
		public Dictionary<uint, Presentation> Presentations {
			get {
				Dictionary<uint, Presentation> presentations = new Dictionary<uint, Presentation>();
				foreach(Presentation p in this.presentations) {
					presentations.Add(p.presentation_id, p);
				}

				return presentations;
			}
		}
		public DbSet<Viewer> viewers {
			get;
			set;
		}
		public Dictionary<uint, SanitizedViewer> Viewers {
			get {
				Dictionary<uint, SanitizedViewer> viewers = new Dictionary<uint, SanitizedViewer>();
				foreach(Viewer v in this.viewers) {
					viewers.Add(v.viewer_id, v.Sanitize());
				}

				return viewers;
			}
		}
		public DbSet<Preference> preferences {
			get;
			set;
		}
		public Dictionary<uint, Dictionary<uint, List<uint>>> GetPreferences(uint viewer_id) {
			var data = new Dictionary<uint, Dictionary<uint, List<uint>>>();
			var fetchedDates = dates.ToList();
			var fetchedBlocks = blocks.ToList();
			foreach(Date date in fetchedDates) {
				foreach(Block block in fetchedBlocks) {
					if(!data.ContainsKey(date.date))
						data.Add(date.date, new Dictionary<uint, List<uint>>());

					data[date.date].Add(block.block_id, GetPreferences(viewer_id, date.date, block.block_id));
				}
			}

			return data;
		}
		public List<uint> GetPreferences(uint viewer_id, uint date, uint block_id) {
			var subset = from thus in preferences
						 where thus.viewer_id == viewer_id
						 orderby thus.order
						 select thus.presentation_id;

			return subset.ToList();
		}
		public DbSet<Schedule> schedule {
			get;
			set;
		}
		public List<Schedule> Schedule {
			get {
				var set  = from thus in schedule
							orderby thus.date, thus.block_id select thus;
				
				return set.ToList();
			}
		}

	}
	
}
