CREATE TABLE [dbo].[Listings] (
    [ListingID]         BIGINT        IDENTITY (1, 1) NOT NULL,
    [Subject]           VARCHAR (255) NOT NULL,
    [CourseNumber]      VARCHAR (255) NOT NULL,
    [InstructionType]   VARCHAR (255) NOT NULL,
    [InstructionMethod] VARCHAR (255) NOT NULL,
    [Section]           VARCHAR (255) NOT NULL,
    [CRN]               VARCHAR (255) NOT NULL,
    [Enroll]            VARCHAR (255) NOT NULL,
    [MaxEnroll]         VARCHAR (255) NOT NULL,
    [CourseTitle]       VARCHAR (255) NOT NULL,
    [Times]             VARCHAR (MAX) NOT NULL,
    [Instructor]        VARCHAR (255) NOT NULL,
    [TermID]            BIGINT        NOT NULL,
    [CreatedDate]       DATETIME      NOT NULL,
    [ModifiedDate]      DATETIME      NOT NULL
);

