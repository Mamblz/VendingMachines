using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;

namespace VendingMachines.ViewModels
{
    public class MainPageViewModel : ReactiveObject
    {
        public ReactiveCommand<Unit, Unit> ToAdminPage { get; }
        public ReactiveCommand<Unit, Unit> ToCompany { get; }
        public ReactiveCommand<Unit, Unit> ToUsers { get; }

        public IEnumerable<ISeries> EfficiencyPieSeries { get; }
        public string WorkingPercent => "75";

        public IEnumerable<ISeries> NetworkPieSeries { get; }

        public ObservableCollection<SummaryItem> SummaryItems { get; } = new();

        public ISeries[] SalesSumSeries { get; }
        public ISeries[] SalesQuantitySeries { get; }
        public Axis[] XAxes { get; }
        public Axis[] YAxes { get; }

        public ObservableCollection<string> News { get; } = new()
        {
            "Терминалы K3Pi получают экзайнер от Google",
            "Неопределенное подразделение от KIT Vending / KIT Shop",
            "Ставки НДС 35 % и 7% для УСН",
            "Релиз новой CRM-системы KIT Shop",
            "Новые модели смежных автоматов от KIT Vending",
            "Получение сертификата PCI DSS 4.0"
        };

        public MainPageViewModel()
        {
            ToAdminPage = ReactiveCommand.Create(NavigateToAdminPage);
            ToCompany = ReactiveCommand.Create(NavigateToCompany);
            ToUsers = ReactiveCommand.Create(NavigateToUsers);

            int working = int.Parse(WorkingPercent);
            int notWorking = 100 - working;
            EfficiencyPieSeries = new ISeries[]
            {
                new PieSeries<double>
                {
                    Values = new double[] { working },
                    Name = "Работают",
                    Fill = new SolidColorPaint(SKColor.Parse("#2563eb")),
                    Stroke = null,
                    DataLabelsSize = 0,
                    IsVisibleAtLegend = false
                },
                new PieSeries<double>
                {
                    Values = new double[] { notWorking },
                    Name = "Не работают",
                    Fill = new SolidColorPaint(SKColor.Parse("#d1d5db")),
                    Stroke = null,
                    DataLabelsSize = 0,
                    IsVisibleAtLegend = false
                }
            };

            NetworkPieSeries = new ISeries[]
            {
                new PieSeries<double>
                {
                    Values = new double[] { 70 },
                    Name = "Исправен",
                    Fill = new SolidColorPaint(SKColor.Parse("#10b981")),
                    Stroke = null,
                    DataLabelsSize = 12,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    IsVisibleAtLegend = true
                },
                new PieSeries<double>
                {
                    Values = new double[] { 20 },
                    Name = "Неисправен",
                    Fill = new SolidColorPaint(SKColor.Parse("#ef4444")),
                    Stroke = null,
                    IsVisibleAtLegend = true
                },
                new PieSeries<double>
                {
                    Values = new double[] { 10 },
                    Name = "На обслуживании",
                    Fill = new SolidColorPaint(SKColor.Parse("#f59e0b")),
                    Stroke = null,
                    IsVisibleAtLegend = true
                }
            };

            SummaryItems.Add(new SummaryItem("Денег в ТА:", "27 959 р."));
            SummaryItems.Add(new SummaryItem("Сдача в ТА:", "12 729 р."));
            SummaryItems.Add(new SummaryItem("Выручка, сегодня:", "11 870 р."));
            SummaryItems.Add(new SummaryItem("Выручка, вчера:", "13 360 р."));
            SummaryItems.Add(new SummaryItem("Инкассировано, сегодня:", "8 145 р."));
            SummaryItems.Add(new SummaryItem("Инкассировано, вчера:", "9 690 р."));
            SummaryItems.Add(new SummaryItem("Обслуживаю ТА, сел./через:", "2 / 2"));

            var dates = new[] { "01.09", "02.09", "03.09", "04.09", "05.09", "06.09", "07.09", "08.09", "09.09", "10.09" };
            var sumValues = new double[] { 5000, 7500, 6000, 11000, 9500, 13000, 12000, 14000, 13500, 15000 };
            var quantityValues = new double[] { 3000, 4500, 5000, 7000, 6800, 9000, 8500, 10500, 11000, 12500 };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = dates,
                    LabelsRotation = 0,
                    TextSize = 10,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 }
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    MinLimit = 0,
                    MaxLimit = 15000,
                    Labeler = value => value.ToString("N0"),
                    TextSize = 10,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 },
                    CustomSeparators = new double[] { 0, 5000, 10000, 15000 }
                }
            };

            SalesSumSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = sumValues,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColor.Parse("#2563eb"), 3),
                    GeometryFill = new SolidColorPaint(SKColor.Parse("#2563eb")),
                    GeometryStroke = null,
                    GeometrySize = 6,
                    LineSmoothness = 0.8,
                    YToolTipLabelFormatter = point => $"{point.Model:N0}"
                }
            };

            SalesQuantitySeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = quantityValues,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColor.Parse("#10b981"), 3),
                    GeometryFill = new SolidColorPaint(SKColor.Parse("#10b981")),
                    GeometryStroke = null,
                    GeometrySize = 6,
                    LineSmoothness = 0.8,
                    YToolTipLabelFormatter = point => $"{point.Model:N0} шт."
                }
            };
        }

        private void NavigateToAdminPage()
        {
            if (MainWindowViewModel.Instance != null)
                MainWindowViewModel.Instance.Uc = new AdminPage();
        }

        private void NavigateToCompany()
        {
            MainWindowViewModel.Instance.Uc = new CompanyPage();
        }

        private void NavigateToUsers()
        {
            if (MainWindowViewModel.Instance != null)
                MainWindowViewModel.Instance.Uc = new UsersPage();
        }
    }

    public class SummaryItem
    {
        public string Key { get; }
        public string Value { get; }
        public SummaryItem(string key, string value) { Key = key; Value = value; }
    }
}