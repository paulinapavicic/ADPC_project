using Microsoft.Win32;
using Project_2.Models;
using Project_2.Services.Data;
using Project_2.Services.Storage;
using Project_2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly XenaScraperService _xenaService;
        private readonly MinioService _minioService;
        private readonly MongoService _mongoService;
        private readonly CohortDataService _cohortDataService;
        private readonly ClinicalDataService _clinicalDataService;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize services
            _xenaService = new XenaScraperService();
            _minioService = new MinioService();
            _mongoService = new MongoService();
            _cohortDataService = new CohortDataService(_xenaService, _minioService, _mongoService);
            _clinicalDataService = new ClinicalDataService(_mongoService);

            LoadCohortsAsync();
            LoadMinioFilesAsync();
            LoadPatientsAsync();
        }

        private async void LoadCohortsAsync()
        {
            try
            {
                var cohorts = await _xenaService.GetAvailableCohortsAsync();
                lstCohorts.ItemsSource = cohorts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cohorts: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadMinioFilesAsync()
        {
            try
            {
                var files = await _minioService.ListObjectsAsync(Models.Constraints.Constraints.MinioBucketName);
                var fileInfos = new List<FileInfo>();

                foreach (var file in files)
                {
                    // This is a simplified example - in a real app, you'd retrieve tags
                    var parts = file.Split('_');
                    fileInfos.Add(new FileInfo
                    {
                        FileName = file,
                        Cohort = parts.Length > 0 ? parts[0] : "Unknown",
                        UploadDate = DateTime.UtcNow // In a real app, retrieve this from MinIO metadata
                    });
                }

                dgMinioFiles.ItemsSource = fileInfos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading MinIO files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadPatientsAsync()
        {
            try
            {
                var patients = await _mongoService.GetUniquePatientsAsync();
                cmbPatients.ItemsSource = patients;

                if (patients.Any())
                    cmbPatients.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading patients: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnScrapeData_Click(object sender, RoutedEventArgs e)
        {
            var actionWindow = new ActionWindow("Scraping Data", "Select cohorts to scrape:");
            if (actionWindow.ShowDialog() == true)
            {
                try
                {
                    var selectedCohorts = actionWindow.SelectedItems;
                    if (selectedCohorts.Count == 0)
                    {
                        MessageBox.Show("No cohorts selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    foreach (var cohort in selectedCohorts)
                    {
                        await _cohortDataService.ProcessCohortAsync(cohort);
                    }

                    MessageBox.Show("Data scraping completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadMinioFilesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during data scraping: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void btnImportClinical_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "TSV files (*.tsv)|*.tsv",
                Title = "Select Clinical Survival Data File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    await _clinicalDataService.ImportClinicalDataAsync(openFileDialog.FileName);
                    MessageBox.Show("Clinical data imported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadPatientsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing clinical data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnVisualize_Click(object sender, RoutedEventArgs e)
        {
            // Switch to the Gene Expression Data tab
            ((TabControl)this.FindName("TabControl")).SelectedIndex = 1;
        }

        private async void lstCohorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optional: Load cohort-specific information when a cohort is selected
        }

        private async void btnProcessCohort_Click(object sender, RoutedEventArgs e)
        {
            if (lstCohorts.SelectedItem is string cohortId)
            {
                try
                {
                    await _cohortDataService.ProcessCohortAsync(cohortId);
                    MessageBox.Show($"Processed cohort {cohortId} successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadMinioFilesAsync();
                    LoadPatientsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing cohort: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a cohort first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void cmbPatients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPatients.SelectedItem is string patientId)
            {
                await LoadPatientDataAsync(patientId);
            }
        }

        private async Task LoadPatientDataAsync(string patientId)
        {
            try
            {
                var patientData = await _mongoService.GetPatientDataAsync(patientId);
                if (!patientData.Any())
                {
                    MessageBox.Show("No data found for selected patient.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var patient = patientData.First();

                // Update gene expression grid
                var geneData = patient.GeneExpressions
                    .Select(kv => new { Gene = kv.Key, Value = kv.Value })
                    .ToList();
                dgGeneExpressions.ItemsSource = geneData;

                // Update clinical data grid
                dgClinicalData.ItemsSource = new List<GeneExpressionClinical> { patient };

                // Generate visualization
                GenerateBarChart(patient);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading patient data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateBarChart(GeneExpressionClinical patient)
        {
            plotCanvas.Children.Clear();

            if (patient?.GeneExpressions == null || !patient.GeneExpressions.Any())
                return;

            // Set margins and chart dimensions
            double margin = 50;
            double canvasWidth = plotCanvas.ActualWidth - (2 * margin);
            double canvasHeight = plotCanvas.ActualHeight - (2 * margin);

            // Get all gene expressions
            var genes = patient.GeneExpressions.Keys.ToList();
            var values = patient.GeneExpressions.Values.ToList();

            // Find min/max for scaling
            double maxValue = values.Max();
            double minValue = values.Min();
            double range = maxValue - minValue;

            // Draw axes
            var xAxis = new Line
            {
                X1 = margin,
                Y1 = canvasHeight + margin,
                X2 = canvasWidth + margin,
                Y2 = canvasHeight + margin,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            var yAxis = new Line
            {
                X1 = margin,
                Y1 = margin,
                X2 = margin,
                Y2 = canvasHeight + margin,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            plotCanvas.Children.Add(xAxis);
            plotCanvas.Children.Add(yAxis);

            // Calculate bar width and spacing
            double barWidth = canvasWidth / genes.Count * 0.8;
            double spacing = canvasWidth / genes.Count * 0.2;

            // Draw bars for each gene
            for (int i = 0; i < genes.Count; i++)
            {
                double normalizedValue = (values[i] - minValue) / (range == 0 ? 1 : range);
                double barHeight = normalizedValue * canvasHeight;

                var bar = new Rectangle
                {
                    Width = barWidth,
                    Height = barHeight,
                    Fill = new SolidColorBrush(GetColorForValue(normalizedValue))
                };

                Canvas.SetLeft(bar, margin + (i * (barWidth + spacing)));
                Canvas.SetTop(bar, margin + canvasHeight - barHeight);

                plotCanvas.Children.Add(bar);

                // Add gene label
                var label = new TextBlock
                {
                    Text = genes[i],
                    FontSize = 10,
                    TextAlignment = TextAlignment.Center,
                    Width = barWidth
                };

                Canvas.SetLeft(label, margin + (i * (barWidth + spacing)));
                Canvas.SetTop(label, margin + canvasHeight + 5);

                plotCanvas.Children.Add(label);
            }

            // Add title
            var title = new TextBlock
            {
                Text = $"Gene Expression for Patient {patient.PatientId}",
                FontSize = 14,
                FontWeight = FontWeights.Bold
            };

            Canvas.SetLeft(title, margin);
            Canvas.SetTop(title, 10);

            plotCanvas.Children.Add(title);
        }

        private Color GetColorForValue(double normalizedValue)
        {
            // Simple color gradient: blue (low) to red (high)
            byte r = (byte)(normalizedValue * 255);
            byte b = (byte)((1 - normalizedValue) * 255);
            return Color.FromRgb(r, 0, b);
        }
    }

    // Helper class for MinIO file display
    public class FileInfo
    {
        public string FileName { get; set; }
        public string Cohort { get; set; }
        public DateTime UploadDate { get; set; }
    }
}

