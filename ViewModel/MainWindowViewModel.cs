using Real_time_Spectrum_Analyzer.Enum;
using Real_time_Spectrum_Analyzer.Functional;
using Real_time_Spectrum_Analyzer.Model;
using Real_time_Spectrum_Analyzer.Service;
using SciChart.Charting.Model.DataSeries;
using System;
using System.Threading;

namespace Real_time_Spectrum_Analyzer.ViewModel
{
    class MainWindowViewModel : ViewModelBase, IPageViewModel
    {
        #region Setting 
        // FIFO Size.
        private const int FifoSize = 800;
        private const int sizeMA = 256;
        private const int sizeFFT = 512;
        #endregion

        // The current time aтв count sample;
        private double t;
        private int i, y;
        // Data for MA.
        private double data_Averge_1;
        private readonly double[] data_Averge_1_temp = new double[sizeMA];
        private double data_Averge_2;
        private readonly double[] data_Averge_2_temp = new double[sizeMA];
        private double data_Averge_3;
        private readonly double[] data_Averge_3_temp = new double[sizeMA];
        // Buffer for data.
        private readonly double[] xBuffer;
        private readonly double[] yBuffer_1;
        private readonly double[] yBuffer_2;
        private readonly double[] yBuffer_3;
        private readonly double[] yBuffer_1_averge;
        private readonly double[] yBuffer_2_averge;
        private readonly double[] yBuffer_3_averge;
        private readonly double[] yBuffer_1_FFT;
        private readonly double[] yBuffer_2_FFT;
        private readonly double[] yBuffer_3_FFT;
        private double[] fftData_X;
        private double[] fftData_Y;
        private double[] fftData_Z;
        private readonly double[] freqs;
        //*** ----------------------------------------------***//
        private readonly AutoResetEvent evtThread = new AutoResetEvent(true);
        private readonly bool isContinue = false;
        //*** ----------------------------------------------***//
        private bool isStartByte = false;
        private Byte countAcceptPackages = 0, countAcceptByte = 0;
        private readonly Byte[] bufferAcceptByte = new Byte[6];

        public MainWindowViewModel()
        {
            StatusRecord = false;
            StatusLabel = "---";

            xBuffer = new double[10];
            yBuffer_1 = new double[10];
            yBuffer_2 = new double[10];
            yBuffer_3 = new double[10];
            yBuffer_1_averge = new double[10];
            yBuffer_2_averge = new double[10];
            yBuffer_3_averge = new double[10];
            yBuffer_1_FFT = new double[sizeFFT];
            yBuffer_2_FFT = new double[sizeFFT];
            yBuffer_3_FFT = new double[sizeFFT];
            freqs = FftSharp.Transform.FFTfreq(400, sizeFFT / 2);

            // Create a DataSeriesSet
            _renderableSeriesX = new XyDataSeries<double, double>();
            _renderableSeriesY = new XyDataSeries<double, double>();
            _renderableSeriesZ = new XyDataSeries<double, double>();
            _renderableSeriesX_Averge = new XyDataSeries<double, double>();
            _renderableSeriesY_Averge = new XyDataSeries<double, double>();
            _renderableSeriesZ_Averge = new XyDataSeries<double, double>();
            _renderableSeriesX_FFT = new XyDataSeries<double, double>();
            _renderableSeriesY_FFT = new XyDataSeries<double, double>();
            _renderableSeriesZ_FFT = new XyDataSeries<double, double>();
            // Get name axis.
            _renderableSeriesX.SeriesName = "AxisX";
            _renderableSeriesY.SeriesName = "AxisY";
            _renderableSeriesZ.SeriesName = "AxisZ";
            _renderableSeriesX_Averge.SeriesName = "AxisX_Averge";
            _renderableSeriesY_Averge.SeriesName = "AxisY_Averge";
            _renderableSeriesZ_Averge.SeriesName = "AxisZ_Averge";
            _renderableSeriesX_FFT.SeriesName = "AxisX_FFT";
            _renderableSeriesY_FFT.SeriesName = "AxisY_FFT";
            _renderableSeriesZ_FFT.SeriesName = "AxisZ_FFT";
            // Set size FIFO for axis.
            _renderableSeriesX.FifoCapacity = FifoSize;
            _renderableSeriesY.FifoCapacity = FifoSize;
            _renderableSeriesZ.FifoCapacity = FifoSize;
            _renderableSeriesX_Averge.FifoCapacity = FifoSize;
            _renderableSeriesY_Averge.FifoCapacity = FifoSize;
            _renderableSeriesZ_Averge.FifoCapacity = FifoSize;
            _renderableSeriesX_FFT.FifoCapacity = FifoSize;
            _renderableSeriesY_FFT.FifoCapacity = FifoSize;
            _renderableSeriesZ_FFT.FifoCapacity = FifoSize;

            Thread readDataFTDI = new Thread(new ThreadStart(ReadDataFTDI))
            {
                IsBackground = true
            };
            readDataFTDI.Start();

            isContinue = true;
            evtThread.Set();
        }

        #region Thread

        private void ReadDataFTDI()
        {
            while (true)
            {
                evtThread.WaitOne();
                FftiDeviceApi.ClearBuffer();

                while (isContinue)
                {
                    UInt32 numBytesAvailable = 0;
                    if (!FftiDeviceApi.GetRxBytesAvailable(ref numBytesAvailable))
                    {
                        break;
                    }

                    if (numBytesAvailable > 0)
                    {
                        ConvertReciveData();
                    }
                }
            }
        }

        #endregion

        #region Command 

        private RelayCommand _windowLoaded;
        public RelayCommand WindowLoaded
        {
            get
            {
                return _windowLoaded ??
                  (_windowLoaded = new RelayCommand(obj =>
                  {
                      if (Status.isRecord)
                      {
                          StatusRecord = true;
                          StatusLabel = "идет запись";
                      }
                      else
                      {
                          StatusRecord = false;
                          StatusLabel = "---";
                      }
                  }));
            }
        }

        #endregion

        #region Property

        public string Name
        {
            get
            {
                return "MainWindow";
            }
        }

        private bool _statusRecord;
        public bool StatusRecord
        {
            get { return _statusRecord; }
            set
            {
                _statusRecord = value;
                OnPropertyChanged("StatusRecord");
            }
        }

        private string _statusLabel;
        public string StatusLabel
        {
            get { return _statusLabel; }
            set
            {
                _statusLabel = value;
                OnPropertyChanged("StatusLabel");
            }
        }


        #endregion

        #region Chart 

        public string MainChart { get; set; }

        private IDataSeries<double, double> _renderableSeriesX;
        private IDataSeries<double, double> _renderableSeriesY;
        private IDataSeries<double, double> _renderableSeriesZ;
        private IDataSeries<double, double> _renderableSeriesX_Averge;
        private IDataSeries<double, double> _renderableSeriesY_Averge;
        private IDataSeries<double, double> _renderableSeriesZ_Averge;
        private IDataSeries<double, double> _renderableSeriesX_FFT;
        private IDataSeries<double, double> _renderableSeriesY_FFT;
        private IDataSeries<double, double> _renderableSeriesZ_FFT;


        public IDataSeries<double, double> RenderableSeriesX
        {
            get { return _renderableSeriesX; }
            set
            {
                _renderableSeriesX = value;
                OnPropertyChanged("RenderableSeriesX");
            }
        }
        public IDataSeries<double, double> RenderableSeriesY
        {
            get { return _renderableSeriesY; }
            set
            {
                _renderableSeriesY = value;
                OnPropertyChanged("RenderableSeriesY");
            }
        }
        public IDataSeries<double, double> RenderableSeriesZ
        {
            get { return _renderableSeriesZ; }
            set
            {
                _renderableSeriesZ = value;
                OnPropertyChanged("RenderableSeriesZ");
            }
        }
        public IDataSeries<double, double> RenderableSeriesX_Averge
        {
            get { return _renderableSeriesX_Averge; }
            set
            {
                _renderableSeriesX_Averge = value;
                OnPropertyChanged("RenderableSeriesX_Averge");
            }
        }
        public IDataSeries<double, double> RenderableSeriesY_Averge
        {
            get { return _renderableSeriesY_Averge; }
            set
            {
                _renderableSeriesY = value;
                OnPropertyChanged("RenderableSeriesY_Averge");
            }
        }
        public IDataSeries<double, double> RenderableSeriesZ_Averge
        {
            get { return _renderableSeriesZ_Averge; }
            set
            {
                _renderableSeriesZ = value;
                OnPropertyChanged("RenderableSeriesZ_Averge");
            }
        }
        public IDataSeries<double, double> RenderableSeriesX_FFT
        {
            get { return _renderableSeriesX_FFT; }
            set
            {
                _renderableSeriesX_FFT = value;
                OnPropertyChanged("RenderableSeriesX_FFT");
            }
        }
        public IDataSeries<double, double> RenderableSeriesY_FFT
        {
            get { return _renderableSeriesY_FFT; }
            set
            {
                _renderableSeriesY_FFT = value;
                OnPropertyChanged("RenderableSeriesY_FFT");
            }
        }
        public IDataSeries<double, double> RenderableSeriesZ_FFT
        {
            get { return _renderableSeriesZ_FFT; }
            set
            {
                _renderableSeriesZ_FFT = value;
                OnPropertyChanged("RenderableSeriesZ_FFT");
            }
        }

        private void ConvertReciveData()
        {
            Byte[] readData = FftiDeviceApi.ReadByte(1);
            if (readData[0] == 0xFF)
            {
                isStartByte = true;
                return;
            }

            if (isStartByte)
            {
                bufferAcceptByte[countAcceptByte++] = readData[0];
                if (countAcceptByte == 6)
                {
                    isStartByte = false;
                    countAcceptByte = 0;

                    xBuffer[countAcceptPackages] = t;
                    yBuffer_1[countAcceptPackages] = DataCalculation.Convert12Bit(bufferAcceptByte[0], bufferAcceptByte[1]);
                    yBuffer_2[countAcceptPackages] = DataCalculation.Convert12Bit(bufferAcceptByte[2], bufferAcceptByte[3]);
                    yBuffer_3[countAcceptPackages] = DataCalculation.Convert12Bit(bufferAcceptByte[4], bufferAcceptByte[5]);

                    yBuffer_1_averge[countAcceptPackages] = data_Averge_1;
                    yBuffer_2_averge[countAcceptPackages] = data_Averge_2;
                    yBuffer_3_averge[countAcceptPackages] = data_Averge_3;

                    if (_modeRound == ModeRound.ArithmeticMean)
                    {
                        data_Averge_1_temp[i] = DataCalculation.ConvertAbs(bufferAcceptByte[0], bufferAcceptByte[1]);
                        data_Averge_2_temp[i] = DataCalculation.ConvertAbs(bufferAcceptByte[2], bufferAcceptByte[3]);
                        data_Averge_3_temp[i] = DataCalculation.ConvertAbs(bufferAcceptByte[4], bufferAcceptByte[5]);
                    }
                    else if (_modeRound == ModeRound.MovingAverage || _modeRound == ModeRound.RootMeanSquare)
                    {
                        data_Averge_1_temp[i] = DataCalculation.Convert12Bit(bufferAcceptByte[0], bufferAcceptByte[1]);
                        data_Averge_2_temp[i] = DataCalculation.Convert12Bit(bufferAcceptByte[2], bufferAcceptByte[3]);
                        data_Averge_3_temp[i] = DataCalculation.Convert12Bit(bufferAcceptByte[4], bufferAcceptByte[5]);
                    }

                    if (_modeRound == ModeRound.ArithmeticMean)
                    {
                        if (t >= (sizeMA - 1))
                        {
                            for (int i = 0; i < sizeMA; i++)
                            {
                                data_Averge_1 += data_Averge_1_temp[i];
                                data_Averge_2 += data_Averge_2_temp[i];
                                data_Averge_3 += data_Averge_3_temp[i];
                            }

                            data_Averge_1 /= sizeMA;
                            data_Averge_2 /= sizeMA;
                            data_Averge_3 /= sizeMA;

                            DataCalculation.ShiftLeft(data_Averge_1_temp);
                            DataCalculation.ShiftLeft(data_Averge_2_temp);
                            DataCalculation.ShiftLeft(data_Averge_3_temp);
                        }
                        else
                        {
                            i += 1;
                        }
                    }

                    if (_modeRound == ModeRound.RootMeanSquare)
                    {
                        if (t >= (sizeMA - 1))
                        {
                            for (int i = 0; i < sizeMA; i++)
                            {
                                data_Averge_1 += Math.Pow(data_Averge_1_temp[i], 2);
                                data_Averge_2 += Math.Pow(data_Averge_2_temp[i], 2);
                                data_Averge_3 += Math.Pow(data_Averge_3_temp[i], 2);
                            }

                            data_Averge_1 /= sizeMA;
                            data_Averge_2 /= sizeMA;
                            data_Averge_3 /= sizeMA;

                            data_Averge_1 = Math.Sqrt(data_Averge_1);
                            data_Averge_2 = Math.Sqrt(data_Averge_2);
                            data_Averge_3 = Math.Sqrt(data_Averge_3);

                            DataCalculation.ShiftLeft(data_Averge_1_temp);
                            DataCalculation.ShiftLeft(data_Averge_2_temp);
                            DataCalculation.ShiftLeft(data_Averge_3_temp);
                        }
                        else
                        {
                            i += 1;
                        }
                    }
                    else if (_modeRound == ModeRound.MovingAverage)
                    {
                        if (t >= 2)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                data_Averge_1 += data_Averge_1_temp[sizeMA - i - 1];
                                data_Averge_2 += data_Averge_2_temp[sizeMA - i - 1];
                                data_Averge_3 += data_Averge_3_temp[sizeMA - i - 1];
                            }

                            data_Averge_1 /= 2;
                            data_Averge_2 /= 2;
                            data_Averge_3 /= 2;

                            DataCalculation.ShiftLeft(data_Averge_1_temp);
                            DataCalculation.ShiftLeft(data_Averge_2_temp);
                            DataCalculation.ShiftLeft(data_Averge_3_temp);
                        }
                        else
                        {
                            i += 1;
                        }
                    }
                    t += 1;

                    yBuffer_1_FFT[y] = yBuffer_1[countAcceptPackages];
                    yBuffer_2_FFT[y] = yBuffer_2[countAcceptPackages];
                    yBuffer_3_FFT[y] = yBuffer_3[countAcceptPackages];

                    if (y >= (sizeFFT - 1))
                    {

                        if (_modeFFT == ModeFFT.SpectralDensity)
                        {
                            // For audio we typically want the FFT amplitude (in dB)
                            fftData_X = FftSharp.Transform.FFTpower(yBuffer_1_FFT);
                            fftData_Y = FftSharp.Transform.FFTpower(yBuffer_2_FFT);
                            fftData_Z = FftSharp.Transform.FFTpower(yBuffer_3_FFT);
                        }
                        else if (_modeFFT == ModeFFT.Amplitude)
                        {
                            // For audio we typically want the FFT amplitude (in dB)
                            fftData_X = FftSharp.Transform.FFTmagnitude(yBuffer_1_FFT);
                            fftData_Y = FftSharp.Transform.FFTmagnitude(yBuffer_2_FFT);
                            fftData_Z = FftSharp.Transform.FFTmagnitude(yBuffer_3_FFT);
                        }

                        DataCalculation.ShiftLeft(yBuffer_1_FFT);
                        DataCalculation.ShiftLeft(yBuffer_2_FFT);
                        DataCalculation.ShiftLeft(yBuffer_3_FFT);

                        using (_renderableSeriesX_FFT.SuspendUpdates())
                        {
                            _renderableSeriesX_FFT.Clear();
                            _renderableSeriesX_FFT.Append(freqs, fftData_X);
                            _renderableSeriesY_FFT.Clear();
                            _renderableSeriesY_FFT.Append(freqs, fftData_Y);
                            _renderableSeriesZ_FFT.Clear();
                            _renderableSeriesZ_FFT.Append(freqs, fftData_Z);
                        }
                    }
                    else
                    {
                        y++;
                    }

                    if (++countAcceptPackages == 10)
                    {
                        countAcceptPackages = 0;
                        OnNewData();
                    }
                }
            }
        }
        private void OnNewData()
        {
            //using (_renderableSeriesX.SuspendUpdates())
            //{
                _renderableSeriesX.Append(xBuffer, yBuffer_1);
                _renderableSeriesY.Append(xBuffer, yBuffer_2);
                _renderableSeriesZ.Append(xBuffer, yBuffer_3);
                _renderableSeriesX_Averge.Append(xBuffer, yBuffer_1_averge);
                _renderableSeriesY_Averge.Append(xBuffer, yBuffer_2_averge);
                _renderableSeriesZ_Averge.Append(xBuffer, yBuffer_3_averge);
            //}
        }

        #endregion

        #region Selector

        ModeRound _modeRound = ModeRound.ArithmeticMean;

        public ModeRound ModeRound
        {
            get { return _modeRound; }
            set
            {
                if (_modeRound == value)
                    return;

                _modeRound = value;
                OnPropertyChanged("ModeRound");
                OnPropertyChanged("IsArithmeticMeanMode");
                OnPropertyChanged("IsMovingAverageMode");
                OnPropertyChanged("IsRootMeanSquare");
            }
        }

        public bool IsArithmeticMeanMode
        {
            get { return ModeRound == ModeRound.ArithmeticMean; }
            set { ModeRound = value ? ModeRound.ArithmeticMean : ModeRound; }
        }

        public bool IsMovingAverageMode
        {
            get { return ModeRound == ModeRound.MovingAverage; }
            set { ModeRound = value ? ModeRound.MovingAverage : ModeRound; }
        }

        public bool IsRootMeanSquare
        {
            get { return ModeRound == ModeRound.RootMeanSquare; }
            set { ModeRound = value ? ModeRound.RootMeanSquare : ModeRound; }
        }

        ModeFFT _modeFFT = ModeFFT.Amplitude;

        public ModeFFT ModeFFT
        {
            get { return _modeFFT; }
            set
            {
                if (_modeFFT == value)
                    return;

                _modeFFT = value;
                OnPropertyChanged("ModeRound");
                OnPropertyChanged("IsAmplitudeMode");
                OnPropertyChanged("IsSpectralDensitMode");
            }
        }

        public bool IsAmplitudeMode
        {
            get { return ModeFFT == ModeFFT.Amplitude; }
            set { ModeFFT = value ? ModeFFT.Amplitude : ModeFFT; }
        }

        public bool IsSpectralDensitMode
        {
            get { return ModeFFT == ModeFFT.SpectralDensity; }
            set { ModeFFT = value ? ModeFFT.SpectralDensity : ModeFFT; }
        }

        #endregion
    }
}