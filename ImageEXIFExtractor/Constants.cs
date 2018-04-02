using System;
using System.Collections.Generic;

namespace ImageEXIFExtractor
{
    public static class Constants
    {
        internal static string CameraMake = "CameraMake";
        internal static string CameraModel = "CameraModel";
        internal static string Orientation = "Orientation";
        internal static string EXIFModifiedDateTime = "EXIFModifiedDateTime";
        internal static string MeteringMode = "MeteringMode";
        internal static string Flash = "Flash";
        internal static string FocalLength = "FocalLength";
        internal static string PixelXDimension = "PixelXDimension";
        internal static string PixelYDimension = "PixelYDimension";
        internal static string FileSource = "FileSource";
        internal static string ExposureMode = "ExposureMode";
        internal static string WhiteBalance = "WhiteBalance";
        internal static string FocalLength35mmEquivalent = "FocalLength35mmEquivalent";
        internal static string SceneCaptureType = "SceneCaptureType";
        internal static string SubjectDistanceRange = "SubjectDistanceRange";
        internal static string EXIFCreatedDateTime = "EXIFCreatedDateTime";
        internal static string ExposureTime = "ExposureTime";
        internal static string Aperture = "Aperture";
        internal static string ISO = "ISO";
        internal static string ExposureCompensation = "ExposureCompensation";


        internal static Dictionary<string, int> ExifDetailTags = new Dictionary<string, int>();
        static Constants()
        {
            try
            {
                ExifDetailTags.Add(CameraMake, 0x10F);
                ExifDetailTags.Add(CameraModel, 0x0110);
                ExifDetailTags.Add(Orientation, 0x0112);
                ExifDetailTags.Add(EXIFModifiedDateTime, 0x0132);
                ExifDetailTags.Add(MeteringMode, 0x9207);
                ExifDetailTags.Add(Flash, 0x9209);
                ExifDetailTags.Add(PixelXDimension, 0xA002);
                ExifDetailTags.Add(PixelYDimension, 0xA003);
                ExifDetailTags.Add(FileSource, 0xA300);
                ExifDetailTags.Add(ExposureMode, 0xA402);
                ExifDetailTags.Add(WhiteBalance, 0xA403);
                ExifDetailTags.Add(FocalLength35mmEquivalent, 0xA405);
                ExifDetailTags.Add(SceneCaptureType, 0xA406);
                ExifDetailTags.Add(SubjectDistanceRange, 0xA40C);
                ExifDetailTags.Add(ExposureCompensation, 0x9204);
                ExifDetailTags.Add(EXIFCreatedDateTime, 0x9003);
                ExifDetailTags.Add(ExposureTime, 0x829A);
                ExifDetailTags.Add(Aperture, 0x829D);
                ExifDetailTags.Add(ISO, 0x8827);
                ExifDetailTags.Add(FocalLength, 0x920A);

            }
            catch (Exception)
            {
                ExifDetailTags = null;
            }

        }
    }
}
