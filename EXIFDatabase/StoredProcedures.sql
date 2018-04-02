CREATE PROCEDURE spAddOrUpdateConfigItem
	@ConfigItemKey NVARCHAR(20),
	@ConfigItemValue NVARCHAR(100)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM [ConfigItems] WHERE ConfigItemKey=@ConfigItemKey)
		BEGIN
			INSERT INTO
				[ConfigItems]
				(ConfigItemKey,ConfigItemValue)
			VALUES
				(@ConfigItemKey,@ConfigItemValue)
		END
	ELSE
		BEGIN
			UPDATE
				[ConfigItems] 
			SET
				ConfigItemValue=@ConfigItemValue
			WHERE
				ConfigItemKey=@ConfigItemKey
		END
END
GO

CREATE PROCEDURE spGetConfigItem
	@ConfigItemKey NVARCHAR(20)
AS
BEGIN
	SELECT
		[ConfigItemKey],[ConfigItemValue]
	FROM
		[ConfigItems]
	WHERE
		[ConfigItemKey]=@ConfigItemKey
END
GO

CREATE PROCEDURE spGetAllConfigItems
AS
BEGIN
	SELECT
		[ConfigItemKey],[ConfigItemValue]
	FROM
		[ConfigItems]
END
GO

CREATE PROCEDURE spAddFolder
	@FolderID BIGINT,
	@FolderPath NVARCHAR(512),
	@IsWatched BIT = 1
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM [FolderSettings] WHERE FolderPath = @FolderPath)
		INSERT INTO [FolderSettings]
			(FolderID,FolderPath,IsWatched)
		VALUES
			(@FolderID,@FolderPath,@IsWatched)
END
GO

CREATE PROCEDURE spGetAllFolders
AS
BEGIN
	SELECT
		[FolderID],[FolderPath],[IsWatched],[ParentFolderID]
	FROM
		[FolderSettings]
END
GO

CREATE PROCEDURE spGetWatchedFolders
	@IsWatched BIT = 1,
	@ParentFolderID BIGINT = NULL
AS
BEGIN
	IF (@ParentFolderID IS NULL)
		SELECT
			[FolderID],[FolderPath],[IsWatched],[ParentFolderID]
		FROM
			[FolderSettings]
		WHERE
			[IsWatched]=@IsWatched
	ELSE
		SELECT
			[FolderID],[FolderPath],[IsWatched],[ParentFolderID]
		FROM
			[FolderSettings]
		WHERE
			[IsWatched]=@IsWatched AND [ParentFolderID] = @ParentFolderID
END
GO

CREATE PROCEDURE spGetAllSubFolders
	@FolderPath NVARCHAR(512)
AS
	DECLARE @FolderID BIGINT
BEGIN
	SELECT
		@FolderID = [FolderID]
	FROM
		[FolderSettings]
	WHERE
		[FolderPath] = @FolderPath;
	
	SELECT
		[FolderID],[FolderPath],[IsWatched]
	FROM
		[FolderSettings]
	WHERE
		[ParentFolderID] = @FolderID;
END
GO

CREATE PROCEDURE spAddOrUpdateFolderSettings
	@FolderID BIGINT,
	@FolderPath NVARCHAR(512),
	@IsWatched BIT,
	@ParentFolderID BIGINT
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM [FolderSettings] WHERE [FolderID] = @FolderID)
		BEGIN
			INSERT INTO [FolderSettings]
				([FolderID],[FolderPath],[IsWatched],[ParentFolderID])
			VALUES
				(@FolderID,@FolderPath,@IsWatched,@ParentFolderID)
		END
	ELSE
		BEGIN
			UPDATE
				[FolderSettings]
			SET
				[FolderPath]=@FolderPath,
				[IsWatched]=@IsWatched,
				[ParentFolderID]=@ParentFolderID
			WHERE
				[FolderID]=@FolderID
		END
END
GO

CREATE PROCEDURE spAddEXIFData
	@ImageID BIGINT,
	@FolderPath NVARCHAR(512),
	@CameraMake NVARCHAR(50),
	@CameraModel NVARCHAR(50),
	@Aperture NVARCHAR(6),
	@ShutterSpeed NVARCHAR(10),
	@ISO SMALLINT,
	@FocalLength NUMERIC(4, 1),
	@Orientation TINYINT,
	@MeteringMode TINYINT,
	@ExposureBias TINYINT,
	@ExposureMode TINYINT,
	@PixelXDimension SMALLINT,
	@PixelYDimension SMALLINT,
	@CreatedDateTime DATETIME,
	@ModifiedDateTime DATETIME
AS
	DECLARE @FolderID BIGINT
	DECLARE @CameraID INT
BEGIN
	SELECT
		@FolderID = [FolderID]
	FROM
		[FolderSettings]
	WHERE
		[FolderPath] = @FolderPath
	IF @FolderID IS NOT NULL
		BEGIN
			IF NOT EXISTS (SELECT 1 FROM [CameraInformation] WHERE [CameraMake] = @CameraMake AND [CameraModel] = @CameraModel)
				BEGIN
					INSERT INTO [CameraInformation]
						([CameraMake],[CameraModel])
					VALUES
						(@CameraMake,@CameraModel)

					SELECT
						@CameraID = [CameraID]
					FROM
						[CameraInformation]
					WHERE
						[CameraMake] = @CameraMake AND [CameraModel] = @CameraModel
				END
			ELSE
				BEGIN
					SELECT
						@CameraID = [CameraID]
					FROM
						[CameraInformation]
					WHERE
						[CameraMake] = @CameraMake AND [CameraModel] = @CameraModel

					INSERT INTO [ImageEXIFInformation]
						([ImageID],[Folder],[Camera],[Aperture],[ShutterSpeed],[ISO],[FocalLength],[Orientation],[MeteringMode],[ExposureMode],[ExposureBias],[PixelXDimension],[PixelYDimension],[CreatedDateTime],[ModifiedDateTime])
					VALUES
						(@ImageID,@FolderID,@CameraID,@Aperture,@ShutterSpeed,@ISO,@FocalLength,@Orientation,@MeteringMode,@ExposureMode,@ExposureBias,@PixelXDimension,@PixelYDimension,@CreatedDateTime,@ModifiedDateTime)
				END
		END
END
GO

CREATE PROCEDURE spAddCameraInformation
	@CameraMake NVARCHAR(50),
	@CameraModel NVARCHAR(50)
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM [CameraInformation] WHERE [CameraMake] = @CameraMake AND [CameraModel] = @CameraModel)
		INSERT INTO [CameraInformation]
			([CameraMake],[CameraModel])
		VALUES
			(@CameraMake,@CameraModel)
END
GO

CREATE PROCEDURE spGetAllCameraInformation
AS
BEGIN
	SELECT
		[CameraID],[CameraMake],[CameraModel]
	FROM
		[CameraInformation]
END
GO

CREATE PROCEDURE spGetCameraInformationFromMake
	@CameraMake NVARCHAR(50)
AS
BEGIN
	SELECT
		[CameraID],[CameraMake],[CameraModel]
	FROM
		[CameraInformation]
	WHERE
		[CameraMake] = @CameraMake
END
GO

CREATE PROCEDURE spGetEXIFInformationForImage
	@ImageID BIGINT
AS
BEGIN
	SELECT
		ie.ImageID,
		fs.FolderPath,
		ci.CameraMake,
		ci.CameraModel,
		ie.Aperture,
		ie.ShutterSpeed,
		ie.ISO,
		ie.FocalLength,
		ie.ExposureMode,
		ie.ExposureBias,
		ie.MeteringMode,
		ie.Orientation,
		ie.PixelXDimension,
		ie.PixelYDimension,
		ie.CreatedDateTime,
		ie.ModifiedDateTime
	FROM
		[ImageEXIFInformation] ie
	INNER JOIN
		[FolderSettings] fs
	ON
		ie.Folder = fs.FolderID
	INNER JOIN
		[CameraInformation] ci
	ON
		ie.Camera = ci.CameraID
	WHERE
		ie.ImageID = @ImageID
END
GO

CREATE PROCEDURE spGetAllEXIFInformationForFolder
	@FolderID BIGINT
AS
BEGIN
	SELECT
		ie.ImageID,
		fs.FolderPath,
		ci.CameraMake,
		ci.CameraModel,
		ie.Aperture,
		ie.ShutterSpeed,
		ie.ISO,
		ie.FocalLength,
		ie.ExposureMode,
		ie.ExposureBias,
		ie.MeteringMode,
		ie.Orientation,
		ie.PixelXDimension,
		ie.PixelYDimension,
		ie.CreatedDateTime,
		ie.ModifiedDateTime
	FROM
		[ImageEXIFInformation] ie
	INNER JOIN
		[FolderSettings] fs
	ON
		ie.Folder = fs.FolderID
	INNER JOIN
		[CameraInformation] ci
	ON
		ie.Camera = ci.CameraID
	WHERE
		ie.Folder = @FolderID
END
GO