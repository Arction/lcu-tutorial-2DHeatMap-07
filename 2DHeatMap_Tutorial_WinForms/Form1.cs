// ------------------------------------------------------------------------------------------------------
// LightningChart® example code:  Simple 2D Heat Map Chart Demo.
//
// If you need any assistance, or notice error in this example code, please contact support@arction.com. 
//
// Permission to use this code in your application comes with LightningChart® license. 
//
// http://arction.com/ | support@arction.com | sales@arction.com
//
// © Arction Ltd 2009-2019. All rights reserved.  
// ------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Windows.Forms;

// Arction namespaces.
using Arction.WinForms.Charting;            // LightningChartUltimate and general types.
using Arction.WinForms.Charting.SeriesXY;   // Series for 2D chart.

namespace HeatMap_WF
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// LightningChart component.
        /// </summary>
        private LightningChartUltimate _chart = null;

        /// <summary>
        /// Reference to Intensity series in chart.
        /// </summary>
        private IntensityGridSeries _heatMap = null;

        /// <summary>
        /// Heat Map rows.
        /// </summary>
        int _rows = 500;

        ///<summary>
        /// Heat Map columns.
        ///</summary>
        int _columns = 500;

        /// <summary>
        /// Minimum Y-axis value.
        /// </summary>
        private const int MinY = 0;

        /// <summary>
        /// Maximum Y-axis value.
        /// </summary>
        private const int MaxY = 480;

        /// <summary>
        /// Minimum X-axis value.
        /// </summary>
        private const int MinX = 0;

        /// <summary>
        /// Maximum X-axis value.
        /// </summary>
        private const int MaxX = 640;

        public Form1()
        {
            InitializeComponent();

            // Create chart.
            CreateChart();

            // Generate data.
            GenerateData(_columns, _rows);

            // Safe disposal of LightningChart components when the form is closed.
            FormClosed += new FormClosedEventHandler(Form_Closed);

            #region Hidden polishing
            // Customize chart.
            CustomizeChart(_chart);
            #endregion
        }

        // Create chart.
        public void CreateChart()
        {
            // Create chart.
            _chart = new LightningChartUltimate();

            // Set chart control into the parent container.
            _chart.Parent = this;
            _chart.Dock = DockStyle.Fill;

            // Disable rendering before updating chart properties to improve performance
            // and to prevent unnecessary chart redrawing while changing multiple properties.
            _chart.BeginUpdate();

            // Set a LegendBox into a chart with vertical layout.
            _chart.ViewXY.LegendBoxes[0].Layout = LegendBoxLayout.Vertical;

            // Configure X- and Y-axis.
            var axisX = _chart.ViewXY.XAxes[0];
            var axisY = _chart.ViewXY.YAxes[0];
            axisX.Title.Text = "X-Axis Position";
            axisX.SetRange(MinX, MaxX);
            axisY.Title.Text = "Y-Axis Position";
            axisY.SetRange(MinY, MaxY);

            // 1. Create a Heat Map instance as IntensityGridSeries.
            _heatMap = new IntensityGridSeries(_chart.ViewXY, axisX, axisY);

            // Set Heat Map's contents and properties.
            _heatMap.LegendBoxUnits = "°C";
            _heatMap.Title.Text = "Heat Map";
            _heatMap.MouseInteraction = false;
            _heatMap.PixelRendering = false;
            _heatMap.SetRangesXY(MinX, MaxX, MinY, MaxY);
            _heatMap.SetSize(_columns, _rows);

            // Create a ValueRangePalette to present Heat Map's data.
            if (_heatMap.ValueRangePalette != null)
                _heatMap.ValueRangePalette.Dispose();
            _heatMap.ValueRangePalette = CreatePalette(_heatMap);

            // Add Heat Map to chart.
            _chart.ViewXY.IntensityGridSeries.Add(_heatMap);

            // Auto-scale X- and Y-axes.
            _chart.ViewXY.ZoomToFit();

            // Call EndUpdate to enable rendering again.
            _chart.EndUpdate();

        }

        // Create a ValueRangePalette to present Heat Map's data.
        private ValueRangePalette CreatePalette(IntensityGridSeries series)
        {
            // 2. Creating palette for IntensityGridSeries.
            var palette = new ValueRangePalette(series);

            // 3. LightningChart has some preset values for palette steps.
            // Clear the preset values from palette before setting new ones.
            foreach (var step in palette.Steps)
            {
                step.Dispose();
            }
            palette.Steps.Clear();

            // 4. Add steps into palette. 
            // Palette is used for presenting data in Heat Map with different colors based on their value.
            palette.Steps.Add(new PaletteStep(palette, Color.Blue, -25));
            palette.Steps.Add(new PaletteStep(palette, Color.DodgerBlue, 0));
            palette.Steps.Add(new PaletteStep(palette, Color.LawnGreen, 25));
            palette.Steps.Add(new PaletteStep(palette, Color.Yellow, 50));
            palette.Steps.Add(new PaletteStep(palette, Color.Orange, 75));
            palette.Steps.Add(new PaletteStep(palette, Color.Red, 100));
            palette.Type = PaletteType.Gradient;
            palette.MinValue = -50;
            
            // Return the created palette.
            return palette;
        }

        // 5. Generate data.
        public void GenerateData(int columns, int rows)
        {
            // Create new IntensityPoint series for data.
            var data = new IntensityPoint[_columns, _rows];

            // Disable rendering before updating chart properties to improve performance
            // and to prevent unnecessary chart redrawing while changing multiple properties.
            _chart.BeginUpdate();

            // Set data values and add them to Heat Map.
            for (int i = 0; i < _columns; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    // Add values to the IntensityPoint series, points are generated by using following function.
                    data[i, j].Value = 30.0 + 20 * Math.Cos(20 + 0.0001 * (double)(i * j)) + 70.0 * Math.Cos((double)(j - i) * 0.01);
                }
            }

            // Add generated data as Heat Map data.
            _heatMap.Data = data;

            // Call EndUpdate to enable rendering again.
            _chart.EndUpdate();

        }

        // Safe disposal of LightningChart components when the form is closed.
        private void Form_Closed(Object sender, FormClosedEventArgs e)
        {
            // Dispose Chart.
            _chart.Dispose();
            _chart = null;

            // Dispose Heat Map.
            _heatMap.Dispose();
            _heatMap = null;

        }

        #region Hidden polishing
        void CustomizeChart(LightningChartUltimate chart)
        {
            chart.Background.Color = Color.FromArgb(255, 30, 30, 30);
            chart.Background.GradientFill = GradientFill.Solid;
            chart.ViewXY.GraphBackground.Color = Color.FromArgb(255, 20, 20, 20);
            chart.ViewXY.GraphBackground.GradientFill = GradientFill.Solid;
            chart.Title.Color = Color.FromArgb(255, 249, 202, 3);
            chart.Title.MouseHighlight = MouseOverHighlight.None;

            foreach (var yAxis in chart.ViewXY.YAxes)
            {
                yAxis.Title.Color = Color.FromArgb(255, 249, 202, 3);
                yAxis.Title.MouseHighlight = MouseOverHighlight.None;
                yAxis.MajorGrid.Color = Color.FromArgb(35, 255, 255, 255);
                yAxis.MajorGrid.Pattern = LinePattern.Solid;
                yAxis.MinorDivTickStyle.Visible = false;
            }

            foreach (var xAxis in chart.ViewXY.XAxes)
            {
                xAxis.Title.Color = Color.FromArgb(255, 249, 202, 3);
                xAxis.Title.MouseHighlight = MouseOverHighlight.None;
                xAxis.MajorGrid.Color = Color.FromArgb(35, 255, 255, 255);
                xAxis.MajorGrid.Pattern = LinePattern.Solid;
                xAxis.MinorDivTickStyle.Visible = false;
                xAxis.ValueType = AxisValueType.Number;
            }
        }
        #endregion
    }
}
