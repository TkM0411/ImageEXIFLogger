using System;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

namespace ImageEXIFExtractor
{
    public class EXIFDetails
    {
        #region Enums
        public enum Orientation
        {
            Horizontal = 1,
            Rotate180Degree = 3,
            ClockwiseRotate90Degree = 6,
            ClockwiseRotate270Degree = 8
        }

        public enum ImageMeteringMode
        {
            Unknown = 0,
            Average = 1,
            CenterWeightedAverage = 2,
            Spot = 3,
            MultiSpot = 4,
            MultiSegment = 5,
            Partial = 6,
            Undefined = 255
        }

        public enum FileSource
        {
            Unknown = 0,
            FilmScanner = 1,
            ReflectionPrintScanner = 2,
            DigitalCamera = 3
        }

        public enum ExposureMode
        {
            Auto = 0,
            Manual = 1,
            AutoBracket = 2,
            NotAvailable = 3
        }

        public enum SceneMode
        {
            Standard = 0,
            Landscape = 1,
            Portrait = 2,
            Night = 3,
            NotAvailable = 4
        }

        public enum SubjectDistanceRange
        {
            Unknown = 0,
            Macro = 1,
            Close = 2,
            Distant = 3
        }

        public enum Dimension
        {
            X = 0,
            Y = 1
        }

        public enum ImageTimeStamps
        {
            Created = 0,
            Modified = 1
        }

        public enum WhiteBalance
        {
            Auto = 0,
            Manual = 1,
            Unknown = 2
        }
        #endregion

        #region Fields
        Image imageFromFile;
        PropertyItem[] imageProperties = null;
        private string cameraMake;
        private string cameraModel;
        private string shutterSpeed;
        private string aperture;
        Orientation imageOrientation;
        private DateTime modifiedDateTimeStamp;
        private DateTime createdDateTimeStamp;
        private ImageMeteringMode meteringMode;
        private SceneMode imageSceneMode;
        private string exposureCompensation;
        private string flash;
        private decimal focalLength;
        private int pixelXDimension;
        private int pixelYDimension;
        private FileSource imageFileSource;
        private int iso;
        private ExposureMode imageExposureMode;
        private WhiteBalance imageWhiteBalance;
        private string focalLength35mmEquivalent;
        private SubjectDistanceRange imageSubjectDistanceRange;
        #endregion

        #region Field Accessors
        public string CameraMake
        {
            get { return this.cameraMake; }
        }
        public string CameraModel
        {
            get { return this.cameraModel; }
        }
        public string Aperture
        {
            get { return this.aperture; }
        }
        public string ShutterSpeed
        {
            get { return this.shutterSpeed; }
        }
        public Orientation ImageOrientation
        {
            get { return imageOrientation; }
            set { imageOrientation = value; }
        }
        public DateTime ModifiedDateTimeStamp
        {
            get { return modifiedDateTimeStamp; }
        }
        public FileSource ImageFileSource
        {
            get { return imageFileSource; }
        }
        public DateTime CreatedDateTimeStamp
        {
            get { return createdDateTimeStamp; }
        }
        public ImageMeteringMode MeteringMode
        {
            get { return meteringMode; }
        }
        public string Flash
        {
            get { return flash; }
        }
        public int PixelXDimension
        {
            get { return pixelXDimension; }
        }
        public decimal FocalLength
        {
            get { return focalLength; }
        }
        public int PixelYDimension
        {
            get { return pixelYDimension; }
        }
        public int ISO
        {
            get { return iso; }
        }
        public SceneMode ImageSceneMode
        {
            get { return imageSceneMode; }
        }
        public string ExposureCompensation
        {
            get { return exposureCompensation; }
        }
        public ExposureMode ImageExposureMode
        {
            get { return imageExposureMode; }
        }
        public WhiteBalance ImageWhiteBalance
        {
            get { return imageWhiteBalance; }
        }
        public string FocalLength35mmEquivalent
        {
            get { return focalLength35mmEquivalent; }
        }
        public SubjectDistanceRange ImageSubjectDistanceRange
        {
            get { return imageSubjectDistanceRange; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Gets all the image EXIF Data.
        /// </summary>
        /// <param name="image"></param>
        public EXIFDetails(Image image)
        {
            try
            {
                this.imageFromFile = image;
                this.imageProperties = this.imageFromFile.PropertyItems;
                GetAllImagePropertyValues();
            }
            catch (Exception)
            {

            }
            finally
            {
                this.imageProperties = null;
                image.Dispose();
            }
        }

        /// <summary>
        /// Gets all the image EXIF Data. Accepts a collection of PropertyItems for the image.
        /// </summary>
        /// <param name="imagePropertyCollection"></param>
        public EXIFDetails(PropertyItem[] imagePropertyCollection)
        {
            try
            {
                if (imagePropertyCollection != null && imagePropertyCollection.Length > 0)
                {
                    this.imageProperties = imagePropertyCollection;
                    GetAllImagePropertyValues();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                this.imageProperties = null;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets all the image property values.
        /// </summary>
        private void GetAllImagePropertyValues()
        {
            try
            {
                this.cameraMake = GetCameraMake();
                this.cameraModel = GetCameraSpecificModel();
                this.aperture = GetApertureData();
                this.shutterSpeed = GetExposureTime();
                this.iso = GetISOInformation();
                this.focalLength = GetFocalLengthInformation();
                this.pixelXDimension = GetPixelDimension(Dimension.X);
                this.pixelYDimension = GetPixelDimension(Dimension.Y);
                this.imageFileSource = GetImageFileSource();
                this.flash = GetFlashInformation();
                this.createdDateTimeStamp = GetImageDateTimeStamps(ImageTimeStamps.Created);
                this.modifiedDateTimeStamp = GetImageDateTimeStamps(ImageTimeStamps.Modified);
                this.exposureCompensation = GetExposureCompensation();
                this.imageSceneMode = GetImageSceneMode();
                this.meteringMode = GetMeteringModeInfo();
                this.imageExposureMode = GetImageExposureMode();
                this.imageWhiteBalance = GetImageWhiteBalance();
                this.imageSubjectDistanceRange = GetImageSubjectDistanceInformation();
                this.focalLength35mmEquivalent = GetFocalLength35mmEquivalent();
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// Gets the Camera Maker Brand
        /// </summary>
        /// <returns></returns>
        private string GetCameraMake()
        {
            string imageTitle = string.Empty;
            try
            {
                PropertyItem imageTitleProperty = GetImageProperty(Constants.CameraMake);
                if (imageTitleProperty.Type == 2)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in imageTitleProperty.Value)
                    {
                        char ch = Convert.ToChar(b);
                        sb.Append(ch);
                    }
                    imageTitle = sb.ToString().Trim();
                }
            }
            catch (Exception)
            {
                imageTitle = null;
            }
            return imageTitle;
        }

        /// <summary>
        /// Gets the F-Stop or Aperture Value.
        /// </summary>
        /// <returns></returns>
        private string GetApertureData()
        {
            string aperture = string.Empty;
            try
            {
                PropertyItem apertureDataProperty = GetImageProperty(Constants.Aperture);
                if (apertureDataProperty!=null && apertureDataProperty.Type == 5)
                {
                    decimal apertureData = 0.00M;
                    double numerator = 0.0;
                    double denominator = 0.0;
                    for (int countNum = 0; countNum < 4; countNum++)
                    {
                        numerator += apertureDataProperty.Value[countNum] * Math.Pow(256, countNum);
                    }
                    for (int countDen = 0; countDen < 4; countDen++)
                    {
                        denominator += apertureDataProperty.Value[countDen + 4] * Math.Pow(256, countDen);
                    }
                    apertureData = Decimal.Divide(Convert.ToDecimal(numerator), Convert.ToDecimal(denominator));
                    aperture = "f/" + Decimal.Round(apertureData, 1).ToString().Trim();
                }
            }
            catch (Exception)
            {
                aperture = null;
            }
            return aperture;
        }

        /// <summary>
        /// Gets the Camera Specific Model.
        /// </summary>
        /// <returns></returns>
        private string GetCameraSpecificModel()
        {
            string cameraSpecificModel = string.Empty;
            try
            {
                PropertyItem cameraModelProperty = GetImageProperty(Constants.CameraModel);
                if (cameraModelProperty.Type == 2)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in cameraModelProperty.Value)
                    {
                        char ch = Convert.ToChar(b);
                        sb.Append(ch);
                    }
                    cameraSpecificModel = sb.ToString().Trim();
                }
            }
            catch (Exception)
            {
                cameraSpecificModel = null;
            }
            return cameraSpecificModel;
        }

        /// <summary>
        /// Gets the Exposure Time or Shutter Speed.
        /// </summary>
        /// <returns></returns>
        private string GetExposureTime()
        {
            string exposureTime = string.Empty;
            try
            {
                PropertyItem exposureTimeProperty = GetImageProperty(Constants.ExposureTime);
                if (exposureTimeProperty.Type == 5)
                {
                    decimal time = 0.00M;
                    double numerator = 0.0;
                    double denominator = 0.0;
                    for (int countNum = 0; countNum < 4; countNum++)
                    {
                        numerator += exposureTimeProperty.Value[countNum] * Math.Pow(256, countNum);
                    }
                    for (int countDen = 0; countDen < 4; countDen++)
                    {
                        denominator += exposureTimeProperty.Value[countDen + 4] * Math.Pow(256, countDen);
                    }
                    time = Decimal.Divide(Convert.ToDecimal(numerator), Convert.ToDecimal(denominator));
                    if (time <= 0.25M)
                    {
                        //exposureTime = Decimal.Round(Convert.ToDecimal(numerator), 0).ToString() + "/" + Decimal.Round(Convert.ToDecimal(denominator), 0).ToString() + " sec";
                        exposureTime = "1/" + Decimal.Round(Reciprocal(time), 0).ToString().Trim() + " sec";
                    }
                    else
                    {
                        exposureTime = Decimal.Round(time, 1).ToString() + " sec";
                    }
                }
            }
            catch (Exception)
            {
                exposureTime = null;
            }
            return exposureTime;
        }

        private decimal Reciprocal(decimal number)
        {
            decimal reciprocalValue=0.00M;
            try
            {
                if (number != 0.00M)
                {
                    reciprocalValue = Decimal.Divide(1.00M, number);
                }
            }
            catch (Exception)
            {
                reciprocalValue = Decimal.MinValue;
            }
            return reciprocalValue;
        }

        /// <summary>
        /// Generic Method to return the specific image property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private PropertyItem GetImageProperty(string propertyName)
        {
            PropertyItem propertyItem = null;
            try
            {
                if (this.imageProperties != null && this.imageProperties.Length > 0)
                {
                    foreach (PropertyItem pr in this.imageProperties)
                    {
                        if (Constants.ExifDetailTags.ContainsKey(propertyName) && pr.Id == Constants.ExifDetailTags[propertyName])
                        {
                            propertyItem = pr;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                propertyItem = null;
            }
            return propertyItem;
        }

        /// <summary>
        /// Gets the ISO Speed of the image.
        /// </summary>
        /// <returns></returns>
        private int GetISOInformation()
        {
            int isoData = 0;
            try
            {
                PropertyItem isoProperty = GetImageProperty(Constants.ISO);
                if (isoProperty != null && isoProperty.Type == 3)
                {
                    double isovalue = 0.00;
                    for (int count = 0; count < isoProperty.Value.Length; count++)
                    {
                        isovalue += isoProperty.Value[count] * Math.Pow(256, count);
                    }
                    isoData = Convert.ToInt32(isovalue);
                }
            }
            catch (Exception)
            {
                isoData = -1;
            }
            return isoData;
        }

        /// <summary>
        /// Gets the actual focal length of the image.
        /// </summary>
        /// <returns></returns>
        private decimal GetFocalLengthInformation()
        {
            decimal focalLengthValue = 0.00M;
            try
            {
                PropertyItem focalLengthProperty = GetImageProperty(Constants.FocalLength);
                if (focalLengthProperty != null && focalLengthProperty.Type == 5)
                {
                    double numerator = 0.0;
                    double denominator = 0.0;
                    for (int countNum = 0; countNum < 4; countNum++)
                    {
                        numerator += focalLengthProperty.Value[countNum] * Math.Pow(256, countNum);
                    }
                    for (int countDen = 0; countDen < 4; countDen++)
                    {
                        denominator += focalLengthProperty.Value[countDen + 4] * Math.Pow(256, countDen);
                    }
                    focalLengthValue = Decimal.Round(Decimal.Divide(Convert.ToDecimal(numerator), Convert.ToDecimal(denominator)));
                }
            }
            catch (Exception)
            {
                focalLengthValue = Decimal.MinValue;
            }
            return focalLengthValue;
        }

        /// <summary>
        /// Gets the image resolution in pixels.
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        private int GetPixelDimension(Dimension dim)
        {
            int pixelCount = 0;
            try
            {
                PropertyItem dimensionPixelCount = null;
                if (dim.Equals(Dimension.X))
                {
                    dimensionPixelCount = GetImageProperty(Constants.PixelXDimension);
                }
                else if (dim.Equals(Dimension.Y))
                {
                    dimensionPixelCount = GetImageProperty(Constants.PixelYDimension);
                }
                if (dimensionPixelCount != null)
                {
                    double pixelCountValue = 0.00;
                    for (int count = 0; count < dimensionPixelCount.Value.Length; count++)
                    {
                        pixelCountValue += dimensionPixelCount.Value[count] * Math.Pow(256, count);
                    }
                    pixelCount = Convert.ToInt32(pixelCountValue);
                }
            }
            catch (Exception)
            {
                pixelCount = -1;
            }
            return pixelCount;
        }

        /// <summary>
        /// Gets Flash information
        /// </summary>
        /// <returns></returns>
        private string GetFlashInformation()
        {
            string flashInfo = string.Empty;
            try
            {
                PropertyItem flashProperty = GetImageProperty(Constants.Flash);
                if (flashProperty != null)
                {
                    if (flashProperty != null && flashProperty.Type == 3)
                    {
                        double flashDoubleValue = 0.00;
                        for (int count = 0; count < flashProperty.Value.Length; count++)
                        {
                            flashDoubleValue += flashProperty.Value[count] * Math.Pow(256, count);
                        }
                        int flashValue = Convert.ToInt32(flashDoubleValue);
                        string binaryEquivalentOfFlash = NumberSystemConverter.ConvertDecimalIntegerToBase(flashValue, NumberSystemConverter.Base.Binary);
                        string eightBitBinaryFlashValue = NumberSystemConverter.GetBinaryInXBitFormat(binaryEquivalentOfFlash, 8);
                        if (eightBitBinaryFlashValue.Substring(eightBitBinaryFlashValue.Length - 1, 1).Equals("0"))
                        {
                            flashInfo = "Off";
                        }
                        else if (eightBitBinaryFlashValue.Substring(eightBitBinaryFlashValue.Length - 1, 1).Equals("1"))
                        {
                            flashInfo = "On";
                        }
                    }
                }
            }
            catch (Exception)
            {
                flashInfo = null;
            }
            return flashInfo;
        }

        /// <summary>
        /// Gets image file source information.
        /// </summary>
        /// <returns></returns>
        private FileSource GetImageFileSource()
        {
            FileSource fSource = new FileSource();
            try
            {
                PropertyItem imgFileSource = GetImageProperty(Constants.FileSource);
                if (imgFileSource != null)
                {

                }
                else
                {
                    fSource = FileSource.Unknown;
                }
            }
            catch (Exception)
            {
                fSource = FileSource.Unknown;
            }
            return fSource;
        }

        /// <summary>
        /// Gets the Image Created/Modified Timestamps
        /// </summary>
        /// <param name="imageDateTimeStamp"></param>
        /// <returns></returns>
        private DateTime GetImageDateTimeStamps(ImageTimeStamps imageDateTimeStamp)
        {
            DateTime dt = new DateTime();
            try
            {
                PropertyItem piImageDateTimeInfo = null;
                if (imageDateTimeStamp.Equals(ImageTimeStamps.Created))
                {
                    piImageDateTimeInfo = GetImageProperty(Constants.EXIFCreatedDateTime);
                }
                else if (imageDateTimeStamp.Equals(ImageTimeStamps.Modified))
                {
                    piImageDateTimeInfo = GetImageProperty(Constants.EXIFModifiedDateTime);
                }
                if (piImageDateTimeInfo != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in piImageDateTimeInfo.Value)
                    {
                        char ch = Convert.ToChar(b);
                        sb.Append(ch);
                    }
                    string[] dateTimeParts = sb.ToString().Trim().Split(':',' ');
                    dt=new DateTime(Convert.ToInt32(dateTimeParts[0].Trim()),Convert.ToInt32(dateTimeParts[1].Trim()),Convert.ToInt32(dateTimeParts[2].Trim()),Convert.ToInt32(dateTimeParts[3].Trim()),Convert.ToInt32(dateTimeParts[4].Trim()),Convert.ToInt32(dateTimeParts[5].Trim()));
                }
            }
            catch (Exception)
            {
                dt = DateTime.MaxValue;
            }
            return dt;
        }

        /// <summary>
        /// Gets the Exposure Compensation information of image.
        /// </summary>
        /// <returns></returns>
        private string GetExposureCompensation()
        {
            string exposureCompensationInfo = string.Empty;
            try
            {
                PropertyItem piExposureCompensation = GetImageProperty(Constants.ExposureCompensation);
                if (piExposureCompensation != null && piExposureCompensation.Type == 10)
                {
                    decimal evComp;
                    decimal num = Decimal.Zero;
                    decimal den;
                    if (piExposureCompensation.Value[3] == 255)
                    {
                        num = Convert.ToDecimal(256 - piExposureCompensation.Value[0]);
                        exposureCompensationInfo = "-";
                    }
                    else if (piExposureCompensation.Value[3] == 0)
                    {
                        num = Convert.ToDecimal(piExposureCompensation.Value[0]);
                        exposureCompensationInfo = "+";
                    }
                    den = Convert.ToDecimal(piExposureCompensation.Value[4]);
                    evComp = Decimal.Divide(num, den);
                    if (evComp.Equals(Decimal.Zero))
                    {
                        exposureCompensationInfo = Decimal.Round(evComp, 1).ToString().Trim() + " step";
                    }
                    else
                    {
                        exposureCompensationInfo += Decimal.Round(evComp, 1).ToString().Trim() + " step";
                    }
                }
            }
            catch (Exception)
            {
                exposureCompensationInfo = null;
            }
            return exposureCompensationInfo;
        }

        /// <summary>
        /// Gets image scene mode information.
        /// </summary>
        /// <returns></returns>
        private SceneMode GetImageSceneMode()
        {
            SceneMode sceneModeInfo = new SceneMode();
            try
            {
                PropertyItem piSceneModeInfo = GetImageProperty(Constants.SceneCaptureType);
                if (piSceneModeInfo != null && piSceneModeInfo.Type == 3)
                {
                    double sceneInfoValue = 0.00;
                    for (int count = 0; count < piSceneModeInfo.Value.Length; count++)
                    {
                        sceneInfoValue += piSceneModeInfo.Value[count] * Math.Pow(256, count);
                    }
                    int sceneInfoData = Convert.ToInt32(sceneInfoValue);
                    sceneModeInfo = (SceneMode)sceneInfoData;
                }
                else
                {
                    sceneModeInfo = SceneMode.NotAvailable;
                }
            }
            catch (Exception)
            {
                sceneModeInfo = SceneMode.NotAvailable;
            }
            return sceneModeInfo;
        }

        /// <summary>
        /// Gets image exposure metering mode information.
        /// </summary>
        /// <returns></returns>
        private ImageMeteringMode GetMeteringModeInfo()
        {
            ImageMeteringMode meteringModeInfo = new ImageMeteringMode();
            try
            {
                PropertyItem piMeteringModeInfo = GetImageProperty(Constants.MeteringMode);
                if (piMeteringModeInfo != null && piMeteringModeInfo.Type == 3)
                {
                    double meteringModeInfoValue = 0.00;
                    for (int count = 0; count < piMeteringModeInfo.Value.Length; count++)
                    {
                        meteringModeInfoValue += piMeteringModeInfo.Value[count] * Math.Pow(256, count);
                    }
                    int meteringModeInfoData = Convert.ToInt32(meteringModeInfoValue);
                    meteringModeInfo = (ImageMeteringMode)meteringModeInfoData;
                }
                else
                {
                    meteringModeInfo = ImageMeteringMode.Unknown;
                }
            }
            catch (Exception)
            {
                meteringModeInfo = ImageMeteringMode.Undefined;
            }
            return meteringModeInfo;
        }

        /// <summary>
        /// Gets the image exosure mode information.
        /// </summary>
        /// <returns></returns>
        private ExposureMode GetImageExposureMode()
        {
            ExposureMode exposureModeInfo = new ExposureMode();
            try
            {
                PropertyItem piExposureModeInfo = GetImageProperty(Constants.ExposureMode);
                if (piExposureModeInfo != null && piExposureModeInfo.Type == 3)
                {
                    double exposureModeInfoValue = 0.00;
                    for (int count = 0; count < piExposureModeInfo.Value.Length; count++)
                    {
                        exposureModeInfoValue += piExposureModeInfo.Value[count] * Math.Pow(256, count);
                    }
                    int exposureModeInfoData = Convert.ToInt32(exposureModeInfoValue);
                    exposureModeInfo = (ExposureMode)exposureModeInfoData;
                }
                else
                {
                    exposureModeInfo = ExposureMode.NotAvailable;
                }
            }
            catch (Exception)
            {
                exposureModeInfo = ExposureMode.NotAvailable;
            }
            return exposureModeInfo;
        }

        /// <summary>
        /// Gets the image white balance information.
        /// </summary>
        /// <returns></returns>
        private WhiteBalance GetImageWhiteBalance()
        {
            WhiteBalance wbInfo = new WhiteBalance();
            try
            {
                PropertyItem piWhiteBalanceInfo = GetImageProperty(Constants.WhiteBalance);
                if (piWhiteBalanceInfo != null && piWhiteBalanceInfo.Type == 3)
                {
                    double whiteBalanceModeInfoValue = 0.00;
                    for (int count = 0; count < piWhiteBalanceInfo.Value.Length; count++)
                    {
                        whiteBalanceModeInfoValue += piWhiteBalanceInfo.Value[count] * Math.Pow(256, count);
                    }
                    int whiteBalanceModeInfoData = Convert.ToInt32(whiteBalanceModeInfoValue);
                    wbInfo = (WhiteBalance)whiteBalanceModeInfoData;
                }
                else
                {
                    wbInfo = WhiteBalance.Unknown;
                }
            }
            catch (Exception)
            {
                wbInfo = WhiteBalance.Unknown;
            }
            return wbInfo;
        }

        /// <summary>
        /// Gets 35mm Equivalent Focal Length.
        /// </summary>
        /// <returns></returns>
        private string GetFocalLength35mmEquivalent()
        {
            string focalLength35mmEquivalent = string.Empty;
            try
            {
                PropertyItem pi35mmFocalLength = GetImageProperty(Constants.FocalLength35mmEquivalent);
                if (pi35mmFocalLength != null && pi35mmFocalLength.Type == 3)
                {
                    double focalLength35mm = 0.00;
                    for (int count = 0; count < pi35mmFocalLength.Value.Length; count++)
                    {
                        focalLength35mm += pi35mmFocalLength.Value[count] * Math.Pow(256, count);
                    }
                    focalLength35mmEquivalent = Math.Round(focalLength35mm, 0).ToString().Trim();
                }
            }
            catch (Exception)
            {
                focalLength35mmEquivalent = null;
            }
            return focalLength35mmEquivalent;
        }

        /// <summary>
        /// Gets Subject Distance Information.
        /// </summary>
        /// <returns></returns>
        private SubjectDistanceRange GetImageSubjectDistanceInformation()
        {
            SubjectDistanceRange subjectDistanceRangeInfo = new SubjectDistanceRange();
            try
            {
                PropertyItem piSubjectDistanceRange = GetImageProperty(Constants.SubjectDistanceRange);
                if (piSubjectDistanceRange != null)
                {

                }
                else
                {
                    subjectDistanceRangeInfo = SubjectDistanceRange.Unknown;
                }
            }
            catch (Exception)
            {
                subjectDistanceRangeInfo = SubjectDistanceRange.Unknown;
            }
            return subjectDistanceRangeInfo;
        }
        #endregion
    }
}