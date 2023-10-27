using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using WPF_MVVM.Helpers;
using WPF_MVVM.Models.Common;
using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Messaging;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using WPF_MVVM.Models.Messages;

namespace WPF_MVVM.Bases
{
    internal partial class FBaseFunc
    {
        public FBaseFunc()
        {
            Cfg = new();
            _log = new(@"Log\Admin", "log");
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            _programVersion = $"{version?.Major}.{version?.Minor}.{version?.Build}";
            Themes = new()
            {
                new Tuple<string, AvalonDock.Themes.Theme>(nameof(AvalonDock.Themes.MetroTheme), new OverrideMetroTheme()),
                new Tuple<string, AvalonDock.Themes.Theme>(nameof(AvalonDock.Themes.Vs2013DarkTheme), new OverrideVS2013DarkTheme())
            };
        }
        public void Init()
        {
            Cfg.LoadSettingFile();
            SetDarkTheme(Cfg.IsDarkTheme);
            GuardScreenSaver(true);
        }
        public void Dispose()
        {
            IsDispose = true;
        }

        public void SetDarkTheme(bool data)
        {
            if (data)
            {
                var lightTheme = Application.Current.Resources.MergedDictionaries.FirstOrDefault(x => x.Source == new Uri(Cfg.GetColorsRd_LightPath()));
                if (lightTheme != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(lightTheme);
                }
                var darkTheme = new ResourceDictionary() { Source = new Uri(Cfg.GetColorsRd_DarkPath()) };
                if (Application.Current.Resources.MergedDictionaries.FirstOrDefault(x => x == darkTheme) != null)
                {
                    return;
                }
                Application.Current.Resources.MergedDictionaries.Add(darkTheme);
            }
            else
            {
                var darkTheme = Application.Current.Resources.MergedDictionaries.FirstOrDefault(x => x.Source == new Uri(Cfg.GetColorsRd_DarkPath()));
                if (darkTheme != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(darkTheme);
                }
                var lightTheme = new ResourceDictionary() { Source = new Uri(Cfg.GetColorsRd_LightPath()) };
                if (Application.Current.Resources.MergedDictionaries.FirstOrDefault(x => x.Source == lightTheme.Source) != null)
                {
                    return;
                }
                Application.Current.Resources.MergedDictionaries.Add(lightTheme);
            }

            Cfg.SetDarkTheme(data);
            InitTheme();
            WeakReferenceMessenger.Default.Send(new EtcMessage(KeyValuePair.Create<string, object?>("ThemeChange", null)));
        }
        public void InitTheme()
        {
            PaletteHelper paletteHelper = new();
            ITheme theme = paletteHelper.GetTheme();

            theme.PrimaryLight = new(theme.PrimaryLight.Color, Cfg.PrimaryForegroundColor);
            theme.PrimaryMid = new(theme.PrimaryMid.Color, Cfg.PrimaryForegroundColor);
            theme.PrimaryDark = new(theme.PrimaryDark.Color, Cfg.PrimaryForegroundColor);
            theme.SecondaryLight = new(theme.SecondaryLight.Color, Cfg.PrimaryForegroundColor);
            theme.SecondaryMid = new(theme.SecondaryMid.Color, Cfg.PrimaryForegroundColor);
            theme.SecondaryDark = new(theme.SecondaryDark.Color, Cfg.PrimaryForegroundColor);

            theme.SetBaseTheme(Cfg.IsDarkTheme ? Theme.Dark : Theme.Light);
            paletteHelper.SetTheme(theme);
            paletteHelper.ChangePrimaryColor(Cfg.PrimaryColor);
            paletteHelper.ChangeSecondaryColor(Cfg.SecondaryColor);
        }
        public Tuple<string, AvalonDock.Themes.Theme> GetAvalonDockTheme()
        {
            if (Cfg.IsDarkTheme)
            {
                return Themes.Last();
            }
            else
            {
                return Themes.First();
            }
        }
        public void GuardScreenSaver(bool value)
        {
            if (Cfg.IsGuardScreenSaver && value)
            {
                SetThreadExecutionState(ExecutionState.ES_ALL);
            }
            else
            {
                SetThreadExecutionState(ExecutionState.ES_CONTINUOUS);
            }
        }
        public void SetLog(string? data)
        {
            if (_log == null || data == null || data.Length <= 0)
            {
                return;
            }

            _log.PRINT_F(data);
        }
        public void SetLog(byte[] data)
        {
            if (_log == null || data == null || data.Length <= 0)
            {
                return;
            }

            _log.PRINT_F(data);
        }
        public async Task SetLogAsync(string? data)
        {
            if (_log == null || data == null || data.Length <= 0)
            {
                return;
            }

            await _log.PRINT_F_Async(data);
        }
        public async Task SetLogAsync(byte[] data)
        {
            if (_log == null || data == null || data.Length <= 0)
            {
                return;
            }

            await _log.PRINT_F_Async(data);
        }

        public static BitmapImage? LoadImage(string? url)
        {
            BitmapImage img = new();
            string defVideo = @$"pack://application:,,,/Resources/Image/video.png";
            string defFile = @$"pack://application:,,,/Resources/Image/files.png";
            try
            {
                if (url == null || url.Length <= 0)
                {
                    return null;
                }
                if (url.Contains(".mp4") || url.Contains(".avi") || url.Contains(".wmv") || url.Contains(".mkv") || url.Contains(".mov"))
                {
                    img.BeginInit();
                    img.UriSource = new Uri(defVideo);
                    img.EndInit();
                    return img;
                }

                HttpClient wc = new();
                var fs = new MemoryStream();
                Task.Run(async () =>
                {
                    var res = await wc.GetAsync(url);
                    await res.Content.CopyToAsync(fs);
                }).Wait();
                img.BeginInit();
                img.StreamSource = fs;
                img.EndInit();
                return img;
            }
            catch
            {
                try
                {
                    img.BeginInit();
                    img.UriSource = new Uri(defFile);
                    img.EndInit();
                    return img;
                }
                catch
                {
                    return null;
                }
            }
        }

        public async Task ExportExcel(ListView data)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            SaveFileDialog dlg = new()
            {
                InitialDirectory = path,
                DefaultExt = "xlsx",
                Filter = "Excel 통합 문서 (*.xlsx)|*.xlsx",
                FileName = $"WPF_MVVM_{DateTime.Now:yyyyMMdd}"
            };
            if (dlg.ShowDialog() == false)
            {
                return;
            }
            string fileName = dlg.FileName;

            SpreadsheetDocument? doc = null;
            try
            {
                doc = SpreadsheetDocument.Create(fileName, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook);
                WorkbookPart workbookPart = doc.AddWorkbookPart();
                workbookPart.Workbook = new();
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new(new SheetData());

                Columns? cols = worksheetPart.Worksheet.GetFirstChild<Columns>();
                bool insertCol = false;
                if (cols == null)
                {
                    cols = new();
                    insertCol = true;
                }

                if (data.View is not GridView gv)
                {
                    return;
                }

                var colHeader = gv.Columns.ToList().FindAll(x => x.Header != null).ToList();
                var colBindingPath = gv.Columns.ToList().FindAll(x => x.Header != null).Select(x => (x.DisplayMemberBinding as Binding)?.Path?.Path ?? string.Empty);
                int colIndex = 1;
                foreach (var col in colHeader)
                {
                    cols.Append(new Column() { Min = (uint)colIndex, Max = (uint)colIndex, Width = 15, CustomWidth = true });
                    colIndex++;
                }

                if (insertCol)
                {
                    worksheetPart.Worksheet.InsertAt(cols, 0);
                }

                uint rowIndex = 2;
                colIndex = 1;
                foreach (var col in colHeader)
                {
                    var buff = col.Header.ToString() ?? string.Empty;
                    var cell = InsertCellInWorksheet(1, colIndex, worksheetPart);
                    cell.CellValue = new(buff);
                    cell.DataType = new(CellValues.String);
                    colIndex++;
                }


                List<Dictionary<string, string>> colValue = new();
                List<object> list = new();
                list.AddRange(data.Items.OfType<object>());
                foreach (var items in list)
                {
                    var buff = from p in items.GetType().GetProperties()
                               select (p.Name.ToString(), p.GetValue(items) is Enum ? EnumHelper.GetDescription(p.GetValue(items) as Enum) : p.GetValue(items)?.ToString() ?? string.Empty);
                    Dictionary<string, string> buffTemp = new();
                    foreach (var buffItems in buff)
                    {
                        buffTemp.Add(buffItems.Item1.ToString(), buffItems.Item2.ToString() ?? string.Empty);
                    }
                    colValue.Add(buffTemp);
                }

                colIndex = 1;
                foreach (var item in colValue)
                {
                    foreach (var col in colBindingPath)
                    {
                        if (col.Length <= 0)
                        {
                            continue;
                        }

                        var buff = item[col] ?? string.Empty;
                        var cell = InsertCellInWorksheet(rowIndex, colIndex, worksheetPart);
                        cell.CellValue = new(buff);
                        cell.DataType = new(CellValues.String);
                        colIndex++;
                    }
                    rowIndex++;
                    colIndex = 1;
                }

                worksheetPart.Worksheet.Save();

                Sheets? sheets = doc?.WorkbookPart?.Workbook.AppendChild<Sheets>(new Sheets());
                Sheet sheet = new()
                {
                    Id = doc?.WorkbookPart?.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Sheet1"
                };
                sheets?.Append(sheet);
                workbookPart.Workbook.Save();
                doc?.Dispose();
                WeakReferenceMessenger.Default.Send(new AlertMessage(true) { Header = "엑셀 저장 완료", Message = "엑셀 저장이 완료 됨." });
                Process.Start(new ProcessStartInfo()
                {
                    FileName = fileName,
                    UseShellExecute = true,
                });
            }
            catch (Exception ex)
            {
                await SetLogAsync(ex.Message);
                doc?.Dispose();
                WeakReferenceMessenger.Default.Send(new AlertMessage(true) { Header = "엑셀 저장 실패", Message = "엑셀 저장이 실패 함." });
            }
        }

        private Cell InsertCellInWorksheet(uint rowIndex, int columnIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet?.GetFirstChild<SheetData>() ?? new();
            string cellReference = EXCEL_ARRAY[columnIndex - 1] + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Any(r => r.RowIndex == rowIndex) == true)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Any(c => c.CellReference?.Value == EXCEL_ARRAY[columnIndex - 1] + rowIndex) == true)
            {
                return row.Elements<Cell>().Where(c => c.CellReference?.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell? refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell?.CellReference?.Value, cellReference, true) > 0 && cell?.CellReference?.Value?.Length >= cellReference.Length)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);
                return newCell;
            }
        }
    }
}
