using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NewWpfImageViewer.Interfaces
{
    public enum Size
    {
        MEDIUM,
        SMALL,
        BIG
    }

    /// <summary>
    /// Интерфейс структуры класса "Изображения", который отвечает за все, что связано с отображеним картинки
    /// </summary>
    /// <remarks>
    /// Отвечает за формирование соурса для контрола в заданном формате,
    /// а так как хранение соурсов - дорого, также отвечает за обнуление соурса и исходника, когда контрол не видно
    /// и загрузку исходника и соурса, когда контрол изображения в зоне прогрузки
    /// </remarks>
    public interface IImageStructure : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Установка позиции на контроле для расчета видимости
        /// </summary>
        System.Windows.Point Position { set; }

        /// <summary>
        /// Маркер, является ли хранимое изображение GIF-анимацией
        /// </summary>
        bool IsAnimation { get; }

        /// <summary>
        /// Желаемый размер изображения
        /// </summary>
        Size Size { get; set; }

        /// <summary>
        /// На основании <see cref="Size"/> возвращает значение высоты возвращаемого изображения
        /// </summary>
        double Height { get; }

        /// <summary>
        /// Добавочная ширина, используемая для заполнения строк изображениями без обрывов
        /// </summary>
        double WidthAdded { get; set; }

        /// <summary>
        /// На основании размеров <see cref="MaxSizedImage"/>, <see cref="WidthAdded"/> и <see cref="Height"/> возвращает значение реальной ширины с сохранением пропорций
        /// </summary>
        double Width { get; }

        /// <summary>
        /// Источник изображения
        /// </summary>
        ImageSource BitmapSource { get; }

        /// <summary>
        /// Обработчик события смены видимости
        /// </summary>
        /// <param name="sender">Syste.Windows.Controls.ScrollViewer</param>
        /// <param name="e"></param>
        void VisabilityChanged(object sender, EventArgs e);

        /// <summary>
        /// Загрузить данные в <see cref="BitmapSource"/> для отображения изображения
        /// </summary>
        void BitmapSourceEnable();

        /// <summary>
        /// Обнулить <see cref="BitmapSource"/> и спрятать изображение
        /// </summary>
        void BitmapSourceDisable();

        string OriginalFilepath { get; }
    }
}
