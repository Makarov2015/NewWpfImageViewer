using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewWpfImageViewer.ClassDir
{
    class DynamicRowFormatter
    {
        /// <summary>
        /// Метод подбирает оптимальную ширину картинок в ряду, переносит при возможности картинку в ряд выше, сужает и расширяет картинки для заполнения пространства
        /// C учетом внешних отступов картинок (считаем что они у всех одинаковые) и наличия скролбара
        /// </summary>
        /// <returns>Возвращает измененный список AutoStackImage с заполненным полем WidthAdded</returns>
        public static void FormatPlate(List<ClassDir.AutoStackImage> images, double wrapPanel)
        {
            // При множестве ресайзов - будет неправильный результат
            foreach (var item in images)
            {
                item.WidthAdded = 0;
            }

            // Наполняем этот список, пока не забьем ряд
            List<ClassDir.AutoStackImage> currentRow = new List<ClassDir.AutoStackImage>();
            // Наполняем этот список заполненными рядами
            List<ClassDir.AutoStackImage> finalRow = new List<ClassDir.AutoStackImage>();

            // Сумма внешних отступов для картинок
            var marginSum = images.First(x => x.ImageControl != null).ImageControl.Margin.Left * 2;

            double panelWidth = wrapPanel;

            for (int i = 0; i < images.Count; i++)
            {
                // Сумма в ряду
                double sumOfCurrent = currentRow.Sum(x => x.Width);
                // Текущий размер картинки
                double currentSize = images[i].Width;
                // Остается места
                double freeSpace = panelWidth - sumOfCurrent - (marginSum * currentRow.Count);

                // Если места не хватает
                if (freeSpace < images[i].Width + marginSum)
                {
                    // Места мало - текущий ряд не меняется в составе, но растягивается на остаток
                    if (freeSpace < currentSize / 2)
                    {
                        // Посчитали надбавку
                        double needToAdd = freeSpace / currentRow.Count;

                        // Добавили надбавку
                        foreach (var predicredRow in currentRow)
                        {
                            predicredRow.WidthAdded = needToAdd;
                        }

                        // И теперь складываем измененные картинки в конечный лист
                        finalRow.AddRange(currentRow.ToList());
                        currentRow.Clear();

                        currentRow.Add(images[i]);
                    }
                    // Места много - в текущий ряд добавляем картинку из следующего, эту картинку удаляем оттуда и текущий ряд сжимаем для 
                    else
                    {
                        // Добавили первую из следующего ряда
                        currentRow.Add(images[i]);

                        // Считаем на сколько надо всех уменьшить
                        var curSize = ((currentRow.Sum(x => x.Width) + marginSum * currentRow.Count()) - panelWidth) / currentRow.Count();

                        // Добавили надбавку
                        foreach (var predicredRow in currentRow)
                        {
                            predicredRow.WidthAdded = curSize * (-1);
                        }

                        finalRow.AddRange(currentRow.ToList());
                        currentRow.Clear();
                    }
                }
                else
                {
                    currentRow.Add(images[i]);

                    // BUG Иногда пропадает последняя картинка
                    if (images[i] == images.Last())
                    {
                        finalRow.AddRange(currentRow.ToList());
                        break;
                    }
                }
            }
        }
    }
}
