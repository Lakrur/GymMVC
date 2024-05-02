using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using GymDomain.Model;
using Microsoft.EntityFrameworkCore;
using Member = GymDomain.Model.Member;

namespace GymInfrastructure.Services
{
    public class CategoryImportService : IImportService<Gym>
    {
        private readonly DbgymContext _context;

        public CategoryImportService(DbgymContext context)
        {
            _context = context;
        }

        private async Task AddClassAsync(IXLRow row, CancellationToken cancellationToken, Gym gym)
        {
            GroupClass groupClass = new GroupClass();
            groupClass.Trainers = new List<Trainer>();
            groupClass.Name = GetGroupClassName(row);
            bool groupClassExists = _context.GroupClasses.Any(g => g.Name == groupClass.Name && g.Room == GetGroupClassRoom(row));
            if (groupClassExists)
            {
                groupClass = _context.GroupClasses.Include(g => g.Members).Include(g => g.Trainers).FirstOrDefault(g => g.Name == groupClass.Name && g.Room == GetGroupClassRoom(row));
            }
            else
            {
                groupClass.Room = GetGroupClassRoom(row);
                groupClass.Schedule = GetGroupClassSchedule(row);
                groupClass.Price = GetGroupClassPrice(row);
                groupClass.GymId = gym.Id;

                if (_context.GroupClasses.FirstOrDefault(g => g.Name == groupClass.Name && g.Room == groupClass.Room) == null)
                {
                    _context.GroupClasses.Add(groupClass);
                }
            }

            bool trainereExists = _context.Trainers.Any(t => t.TrainerName == row.Cell(1).Value.ToString());
            if (!trainereExists)
            {
                if (row.Cell(1).Value.ToString() != "")
                {
                    Trainer trainer = new Trainer { TrainerName = row.Cell(1).Value.ToString(), Phone = row.Cell(2).Value.ToString(), Email = row.Cell(3).Value.ToString(), GymId = gym.Id, GroupClasses = new List<GroupClass>() };
                    _context.Trainers.Add(trainer);

                    groupClass.Trainers.Add(trainer);
                    trainer.GroupClasses.Add(groupClass);
                }
            }

            else if (trainereExists)
            {
                Trainer trainer = _context.Trainers.FirstOrDefault(t => t.TrainerName == row.Cell(1).Value.ToString());


                trainer.GroupClasses.Add(groupClass);

                groupClass.Trainers.Add(trainer);


            }


            
            _context.SaveChanges();
            await GetMembersAsync(row, groupClass, cancellationToken);
        }

        private static string GetGroupClassName(IXLRow row)
        {
            return row.Cell(4).Value.ToString();
        }

        private static int GetGroupClassRoom(IXLRow row)
        {
            return (int)row.Cell(5).Value.GetNumber();
        }


        private static string GetGroupClassSchedule(IXLRow row)
        {
            return row.Cell(6).Value.ToString();
        }

        private static int GetGroupClassPrice(IXLRow row)
        {
            return (int)row.Cell(7).Value.GetNumber();
        }



        private async Task GetMembersAsync(IXLRow row, GroupClass groupclass, CancellationToken cancellationToken)

        {
            int column = 8; // Start reading from column 5

            while (true)
            {
                var cellValue = row.Cell(column).Value.ToString();

                if (string.IsNullOrEmpty(cellValue))
                {
                    break; // Exit the loop if the cell value is empty or null
                }

                var memberName = cellValue;

                // Check if the author already exists in the database
                var member = await _context.Members.FirstOrDefaultAsync(aut => aut.Name == memberName, cancellationToken);

                if (member == null)
                {
                    member = new Member();
                    member.Name = memberName;
                    member.Phone = row.Cell(column + 1).Value.ToString();
                    member.SubscriptionId = (int)row.Cell(column + 2).Value.GetNumber();

                    member.Email = row.Cell(column + 3).Value.ToString();
                    member.GymId = groupclass.GymId;
                    _context.Members.Add(member);
                    member.GroupClasses.Add(groupclass);
                }
                if (member.GymId != groupclass.GymId)
                {
                    member.GymId = groupclass.GymId;
                }
                if (!groupclass.Members.Contains(member))
                {
                    groupclass.Members.Add(member);
                }

                column += 4;
            }
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Cannot be transmitted", nameof(stream));
            }

            using (XLWorkbook workBook = new XLWorkbook(stream))
            {
                foreach (IXLWorksheet worksheet in workBook.Worksheets)
                {
                    var gymName = worksheet.Name;
                    var gym = await _context.Gyms.FirstOrDefaultAsync(cat => cat.Adress == gymName, cancellationToken);
                    if (gym == null)
                    {
                        gym = new Gym();
                        gym.Adress = gymName;
                        var row1 = worksheet.Row(1);
                        gym.Schedule = row1.Cell(2).Value.ToString();
                        gym.Equipment = row1.Cell(4).Value.ToString();
                        _context.Gyms.Add(gym);

                    }

                    _context.SaveChanges();
                    foreach (var row in worksheet.RowsUsed().Skip(2))
                    {
                        await AddClassAsync(row, cancellationToken, gym);
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}