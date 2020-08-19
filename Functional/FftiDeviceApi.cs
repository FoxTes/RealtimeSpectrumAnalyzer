using FTD2XX_NET;
using System;
using System.Windows;

namespace Real_time_Spectrum_Analyzer.Functional
{
    static class FftiDeviceApi
    {
        static FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
        // Создание нового экземпляр класса устройства FTDI.
        static readonly FTDI myFtdiDevice = new FTDI();

        public static Boolean OpenPortToDevice()
        {
            UInt32 ftdiDeviceCount = 0;

            // Определение количества устройств FTDI, подключенных сейчас к компьютеру.
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            // Проверка результата вызова GetNumberOfDevices.
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                //MessageBox.Show("Number of FTDI devices: "
                //                + ftdiDeviceCount.ToString());
            }
            else
            {
                // Сообщеяем об ошибке.
                //MessageBox.Show("Failed to get number of devices (error "
                //                + ftStatus.ToString() + ")");
                return false;
            }

            // Если подключено хотя бы одно устройство.
            if (ftdiDeviceCount != 0)
            {
                // Пытаемся открыть устройство по дискриптору.
                ftStatus = myFtdiDevice.OpenByDescription("PGS1");
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    // Сообщеяем об ошибке.
                    //MessageBox.Show("Failed to open device (error "
                    //                + ftStatus.ToString() + ")");
                    return false;
                }
                else
                {
                    //// Сообщяем об успешном покдлючении.
                    //MessageBox.Show("Connect to open PGS1");
                }
            }
            else
            {
                return false;
            }

            // Настройка параметров устройства, установить скорость на 76800 бод.
            ftStatus = myFtdiDevice.SetBaudRate(76800);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Сообщеяем об ошибке.
                MessageBox.Show("Проблема в установке скорости (ошибка "
                                + ftStatus.ToString() + ")");
                return false;
            }

            // Установить формат фрейма - сколько бит данных, стоп-битов, четность.
            ftStatus = myFtdiDevice.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8,
                                                           FTDI.FT_STOP_BITS.FT_STOP_BITS_1,
                                                           FTDI.FT_PARITY.FT_PARITY_NONE);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Сообщеяем об ошибке.
                MessageBox.Show("Не получилось настроить параметры фрейма (ошибка "
                                + ftStatus.ToString() + ")");
                return false;
            }

            // Установить таймаут чтения на 3 секунды, таймаут записи на бесконечность.
            ftStatus = myFtdiDevice.SetTimeouts(3000, 0);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Сообщеяем об ошибке.
                MessageBox.Show("Не получилось установить таймауты (ошибка "
                                + ftStatus.ToString() + ")");
                return false;
            }
            return true;
        }

        public static UInt32 GetNumberOfDevices()
        {
            UInt32 deviceCount = 0;

            // Determine the number of FTDI devices connected to the machine
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref deviceCount);

            // Check status
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                return 0;
            }
            return deviceCount;
        }

        public static Boolean WriteByte(Byte data)
        {
            // Запись массива в устройство.
            Byte[] dataToWrite = new Byte[1] { data };
            UInt32 numBytesWritten = 0;

            // Отрпавляем массив в устройство.
            ftStatus = myFtdiDevice.Write(dataToWrite,
                                          dataToWrite.Length,
                                          ref numBytesWritten);
            // Проверка результата вызова Write.
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Сообщеяем об ошибке.
                MessageBox.Show("Неудачная запись в устройство (ошибка "
                                + ftStatus.ToString() + ")");
                return false;
            }
            else
            {
                //MessageBox.Show("Количество записанных байт "
                //                + numBytesWritten.ToString());
            }
            return true;
        }

        public static Boolean WriteByte(ref Byte[] dataToWrite)
        {
            UInt32 numBytesWritten = 0;

            // Отрпавляем массив в устройство.
            ftStatus = myFtdiDevice.Write(dataToWrite,
                                          dataToWrite.Length,
                                          ref numBytesWritten);
            // Проверка результата вызова Write.
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Сообщеяем об ошибке.
                MessageBox.Show("Неудачная запись в устройство (ошибка "
                                + ftStatus.ToString() + ")");
                return false;
            }
            else
            {
                //MessageBox.Show("Количество записанных байт "
                //                + numBytesWritten.ToString());
            }
            return true;
        }

        public static Byte[] ReadByte(UInt32 countByte)
        {
            Byte[] readData = new Byte[countByte];
            UInt32 numBytesRead = 0;
            ftStatus = myFtdiDevice.Read(readData,
                                         countByte,
                                         ref numBytesRead);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
#if DEBUG
                // Сообщеяем об ошибке.
                MessageBox.Show("Не получилось прочитать данные (ошибка "
                                 + ftStatus.ToString() + ")");
#endif
                return new Byte[0];
            }
            return readData;
        }

        public static Byte ReadByte()
        {
            Byte[] readData = new Byte[1];
            UInt32 numBytesRead = 0;
            ftStatus = myFtdiDevice.Read(readData, 1, ref numBytesRead);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
#if DEBUG
                // Сообщеяем об ошибке.
                MessageBox.Show("Не получилось прочитать данные (ошибка "
                                 + ftStatus.ToString() + ")");
#endif
                return new Byte();
            }
            return readData[0];
        }

        public static Boolean ClearBuffer()
        {
            ftStatus = myFtdiDevice.Purge(FTDI.FT_PURGE.FT_PURGE_RX | FTDI.FT_PURGE.FT_PURGE_TX);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
#if DEBUG
                // Сообщеяем об ошибке.
                MessageBox.Show("Не удалось очистить буферы "
                                + "устройства (ошибка "
                                + ftStatus.ToString() + ")");
#endif
                return false;
            }
            return true;
        }

        public static Boolean GetRxBytesAvailable()
        {
            UInt32 numBytesAvailable = 0;
            ftStatus = myFtdiDevice.GetRxBytesAvailable(ref numBytesAvailable);

            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                return false;
            }
            return true;
        }

        public static Boolean GetRxBytesAvailable(ref UInt32 outByte)
        {
            UInt32 numBytesAvailable = 0;
            ftStatus = myFtdiDevice.GetRxBytesAvailable(ref numBytesAvailable);

            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
#if DEBUG
                // Сообщеяем об ошибке.
                MessageBox.Show("Не получилось проверить количество доступных "
                                + "для чтения байт (ошибка "
                                + ftStatus.ToString() + ")");
#endif
                outByte = 0;
                return false;
            }
            outByte = numBytesAvailable;
            return true;
        }

        public static Boolean IsOpen
        {
            get { return myFtdiDevice.IsOpen; }
        }

        public static Boolean Close()
        {
            ftStatus = myFtdiDevice.Close();
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
#if DEBUG
                // Сообщеяем об ошибке.
                MessageBox.Show("Не удалось закрыть соединение с "
                                + "устройством (ошибка "
                                + ftStatus.ToString() + ")");
#endif
                return false;
            }
            return true;
        }
    }
}
