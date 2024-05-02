using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using GymDomain.Model;
using Microsoft.EntityFrameworkCore;

namespace GymInfrastructure.Services
{
    public class CategoryExportService : IExportService<GymDomain.Model.Gym>
    {
        private const string RootWorksheetName = "";

        private static readonly IReadOnlyList<string> HeaderNames =
            new string[]
            {
                "Ім'я тренера",
                "Телефон тренера",
                "Email тренера",

                "Назва",
                "Кімната",
                "Розклад",
                "Ціна,$",
                "Учасники",
            };
        private static readonly IReadOnlyList<string> Header2Names =
           new string[]
           {
                "Розклад",
                "Інвентар",
           };
        private readonly DbgymContext _context;
        private static void WriteHeader(IXLWorksheet worksheet)
        {
            worksheet.Cell(1, 1).Value = Header2Names[0];
            worksheet.Cell(1, 3).Value = Header2Names[1];

            for (int columnIndex = 0; columnIndex < HeaderNames.Count; columnIndex++)
            {
                worksheet.Cell(2, columnIndex + 1).Value = HeaderNames[columnIndex];
            }
            worksheet.Row(2).Style.Font.Bold = true;
        }

        private void WriteClass(IXLWorksheet worksheet, GroupClass groupclass, int rowIndex)
        {
            var columnIndex = 1;
            var trainer = groupclass.Trainers.FirstOrDefault();
            if (trainer != null)
            {
                worksheet.Cell(rowIndex, columnIndex++).Value = trainer.TrainerName;
                worksheet.Cell(rowIndex, columnIndex++).Value = trainer.Phone;
                worksheet.Cell(rowIndex, columnIndex++).Value = trainer.Email;
            }
            else
            {
                columnIndex += 3;
            }
            worksheet.Cell(rowIndex, columnIndex++).Value = groupclass.Name;

            worksheet.Cell(rowIndex, columnIndex++).Value = groupclass.Room;
            worksheet.Cell(rowIndex, columnIndex++).Value = groupclass.Schedule;
            worksheet.Cell(rowIndex, columnIndex++).Value = groupclass.Price;


            var members = groupclass.Members;

            //groupclass.AuthorBooks.ToList();
            foreach (var ab in members)
            {
                worksheet.Cell(rowIndex, columnIndex++).Value = ab.Name;
                worksheet.Cell(rowIndex, columnIndex++).Value = ab.SubscriptionId;
                worksheet.Cell(rowIndex, columnIndex++).Value = ab.Phone;
                worksheet.Cell(rowIndex, columnIndex++).Value = ab.Email;

            }

        }

        private void WriteBooks(IXLWorksheet worksheet, ICollection<GroupClass> groupclass)
        {
            WriteHeader(worksheet);
            int rowIndex = 3;
            foreach (var groupclass1 in groupclass)
            {

                WriteClass(worksheet, groupclass1, rowIndex);
                rowIndex++;
            }
        }

        private void WriteCategories(XLWorkbook workbook, ICollection<Gym> gyms)
        {
            //для усіх категорій формуємо сторінки
            foreach (var cat in gyms)
            {

                if (cat is not null)
                {
                    var worksheet = workbook.Worksheets.Add(cat.Adress);
                    worksheet.Cell(1, 2).Value = cat.Schedule;
                    worksheet.Cell(1, 4).Value = cat.Equipment;
                    WriteBooks(worksheet, cat.GroupClasses.ToList());
                }
            }
        }

        public CategoryExportService(DbgymContext context)
        {
            _context = context;
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanWrite)
            {
                throw new ArgumentException("Input stream is not writable");
            }

            var gyms = await _context.Gyms
                .Include(g => g.GroupClasses)
                 .ThenInclude(g => g.Trainers)
                .Include(g => g.GroupClasses)
                .ThenInclude(g => g.Members)
                .ToListAsync(cancellationToken);

            var workbook = new XLWorkbook();

            WriteCategories(workbook, gyms);
            workbook.SaveAs(stream);
        }

    }
}


