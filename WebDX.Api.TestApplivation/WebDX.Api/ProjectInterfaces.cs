using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using System.ComponentModel;

namespace WebDX.Api
{
    //TODO: Remove obsolete properties: PerformOCR, DataMask and DataType.
    /// <summary>
    /// The type of data a field will contain. Used for backwards compatibility. Currently, all fields are treated as Text fields.
    /// </summary>
    [OutputPrefix("dt", "1.0.0.0", "1.3.9.9", "WDEDataType")]
	public enum WDEDataType {
        [VersionEnumFilterAttribute("1.0.0.0", "1.3.9.9", "Text", "WDEDataType")]
        Text,
        [VersionEnumFilterAttribute("1.0.0.0", "1.3.9.9", "Number", "WDEDataType")]
        Number,
        [VersionEnumFilterAttribute("1.0.0.0", "1.3.9.9", "DateTime", "WDEDataType")]
        DateTime,
        [VersionEnumFilterAttribute("1.0.0.0", "1.3.9.9", "Currency", "WDEDataType")]
        Currency,
        [VersionEnumFilterAttribute("1.0.0.0", "1.3.9.9", "YesNo", "WDEDataType")]
        YesNo};
	/// <summary>
	/// The keyboard emulation mode.
	/// </summary>
	public enum WDEKeyMode {
		/// <summary>
		/// The standard 101 US keyboard.
		/// </summary>
        [Rename("Normal Keyboard")]
        Normal,
		/// <summary>
		/// Emulate an IBM 029 keypunch keyboard.
		/// </summary>
        [Rename("Embedded 029")]
        Mode029,
		/// <summary>
		/// Emulate an Embedded 10 key keyboard.
		/// </summary>
        [Rename("Embedded 10-key")]
        Mode10Key
	};

	/// <summary>
	/// The current data entry mode.
	/// </summary>
	public enum WDEEntryMode {
		/// <summary>
		/// New data entry mode.
		/// </summary>
		Entry,
		/// <summary>
		/// Verification of previously keyed data mode.
		/// </summary>
		Verify,
		/// <summary>
		/// Flag review/Final Review mode. Obsolete - use OmniPass instead.
		/// </summary>
        [Obsolete("Use OmniPass instead.")]
		Review,
		/// <summary>
		/// QIVerify mode.
		/// </summary>
		QI,
		/// <summary>
		/// Double-Entry mode. Obsolete - use Verify with the FlagVerifyErrors session option set.
		/// </summary>
        [Obsolete("Use Verify with the FlagVerifyErrors session option set.")]
		DblEntry,
		/// <summary>
		/// Compare mode. Obsolete - use OmniPass instead.
		/// </summary>
        [Obsolete("Use OmniPass instead.")]
		Compare,
		/// <summary>
		/// FocusAudit mode. Obsolete - use OmniPass instead.
		/// </summary>
        [Obsolete("Use OmniPass instead.")]
		FocusAudit,
        /// <summary>
        /// OmniPass mode. Formerly called OnePass mode.
        /// </summary>
        OmniPass
	};    

    /// <summary>
    /// The script language to use with this project. Only .Net languages are supported.
    /// </summary>
    public enum WDEScriptLanguage {
		/// <summary>
		/// DelphiScript language. For backwards compatibility only. No longer supported.
		/// </summary>
        [Browsable(false)]
		DelphiScript,
		/// <summary>
		/// VBScript language. For backwards compatibility only. No longer supported.
		/// </summary>
        [Browsable(false)]
		VBScript,
		/// <summary>
		/// JavaScript language. For backwards compatibility only. No longer supported.
		/// </summary>
        [Browsable(false)]
		JavaScript,
		/// <summary>
		/// C# Language.
		/// </summary>
        [Description("The C# language.")]
		CSharpNet,
		/// <summary>
		/// VB.Net Language.
		/// </summary>
        [Description("The VB.Net language.")]
		VBNet,
        /// <summary>
        /// The Chrome pascal language.
        /// </summary>
        [Browsable(false)]
        [Description("The Chrome pascal language.")]
        Chrome
	};

	/// <summary>
	/// Defines the type of session used during data entry. Formerly ViewMode.
	/// </summary>
	[Flags]
    [IgnoreFlags]
	public enum WDESessionType
	{
        [Browsable(false)]
		None = 0,
        FullForm = 1,
        [Browsable(false)]
        Field = 2,
        [Rename("Continuous")]
        PhotoStitch = 4,
        [Browsable(false)]
		AlphaRepair = 8,
        [Browsable(false)]
        NumericRepair = 16,
        [Browsable(false)]
        AlphaNumRepair = 32,
        [Browsable(false)]
        OnePass = 64,
        Indexing = 128
	};

	/// <summary>
	/// Session option flags
	/// </summary>
	[Flags]
	public enum WDESessionOption : long
	{
		/// <summary>
		/// No options.
		/// </summary>
		None = 0x0000000,
		/// <summary>
		/// Allow the entire batch to be rejected.
		/// </summary>
        [Description("Allow the entire batch to be rejected during data entry.")]
		AllowBatchReject = 0x00000001,
		/// <summary>
		/// Allow the floating keying window to be displayed. For backwards compatibility. No longer supported.
		/// </summary>
        [Browsable(false)]
		AllowFloatingWindow = 0x00000002,
		/// <summary>
		/// Force the keyer to view all images before completing a document.
		/// </summary>
        [Description("Force the keyer to look at all images before being allowed to leave a document.")]
		ReviewAllImages = 0x00000004,
		/// <summary>
		/// Show all keyable zones in FullForm or ShowImage mode.
		/// </summary>
        [Browsable(false)]
		ShowWorkZones = 0x00000008,
		/// <summary>
		/// Show the overlay by default.
		/// </summary>
        [Description("Show the overlay for a document by default.")]
		ShowOverlay = 0x00000010,
		/// <summary>
		/// Allow non-key fields to be modified.
		/// </summary>
        [Browsable(false)]
		AllowModifyNonKeyables = 0x00000020,
		/// <summary>
		/// Allow the document to be rejected.
		/// </summary>
        [Description("Allow documents to be rejected.")]
		AllowDocReject = 0x00000040,
		/// <summary>
		/// Auto-save each document as it is completed. Used to recover from power failures.
		/// </summary>
        [Description("Automatically save (cache to the server) the data file after each document is completed.")]
		AutoSave = 0x00000080,
		/// <summary>
		/// Allow the user to change the image type on the fly. This changes the zones used.
		/// </summary>
        [Browsable(false)]
		AllowImageTypeChange = 0x00000100,
		/// <summary>
		/// Allows verify to be at the field level instead of character level. Only applies to verify modes.
		/// </summary>
        [Browsable(false)]
		AllowFieldVerify = 0x00000200,
		/// <summary>
		/// Allows data to be duplicated from previous forms and records.
		/// </summary>
        [Description("Allow data duplication with a single keystroke.")]
		AllowDataDupe = 0x00000400,
		/// <summary>
		/// Removes the requirement to re-verify data in Review mode.
		/// </summary>
        [Browsable(false)]
		DisableVerifyInReview = 0x00000800,
        /// <summary>
        /// Flags the field rather than prompting the user when a compare error happens in Verify mode.
        /// </summary>
        [Description("Flag fields on verify errors rather than forcing a correction.")]
        FlagVerifyErrors = 0x00001000,
        /// <summary>
        /// Causes OnePass mode to only process character repairs.
        /// </summary>
        [Description("OmniPass mode will only stop on fields that need character repair.")]
        [Rename("OmniPassCharRepOnly")]
        OnePassCharRepOnly = 0x00002000,
        /// <summary>
        /// Forces all changes to be verified in OmniPass mode.
        /// </summary>
        [Description("Forces all changes to be verified by double-keying during OmniPass mode.")]
        [Rename("OmniPassVerifyAll")]
        OnePassVerifyAll = 0x00004000,
        /// <summary>
        /// Forces image thumbnails to be rendered in lower quality to improve performance.
        /// </summary>
        [Browsable(false)]
        [Description("Forces thumbnails to be rendered in lower quality to improve performance.")]
        LowResThumbnails = 0x00008000,
        /// <summary>
        /// Enables keying of Validated fields during QIVerify.
        /// </summary>
        [Browsable(false)]
        QIVerifyValidated = 0x00010000,
		/// <summary>
		/// Displays OCR rects as zones before project defined zones.
		/// </summary>
        [Description("Displays OCR-captured rectangles (if they are specified) instead of project-defined zones.")]
		PreferOCRRects = 0x00020000,
		/// <summary>
		/// Loops repeating zones back to the top of the detail grid for subsequent pages of the same form.
		/// </summary>
        [Description("Loops repeating zones back to the top of the detail grid for subsequent pages of the same form.")]
		LoopZones = 0x00040000,
		/// <summary>
		/// Show only the portion of the image in the current zone hiding the rest of the image from view.
		/// </summary>
		[Description("Show only the portion of the image in the current zone hiding the rest of the image from view.")]
		ZonesOnly = 0x00080000,
        /// <summary>
        /// Replaces NULL value of a field to empty string (blank) in Entry mode and provides blind pass entry on fields with NULL value in Verify mode.
        /// </summary>
        [Description("Replaces NULL value of a field to empty string (blank) in Entry mode and provides blind pass entry on fields with NULL value in Verify mode.")]
        MakeNullBlank = 0x00100000,
        /// <summary>
        /// Allows F4 to perform field correction with(double keying) or without verification(single keying).
        /// </summary>
        [Description("Allows F4 to perform field correction with(double keying) or without verification(single keying).")]
        VerifyFieldCorrect = 0x00200000,
        /// <summary>
        /// Displays the field value when F4 is hit and allows to append or change the field value without verification(single keying).
        /// </summary>
        [Description("Displays the field value when F4 is hit and allows to append or change the field value without verification(single keying).")]
        AllowFieldCorrectEdit = 0x00400000,
        /// <summary>
        /// Displays the field values and allows the value to be appended or changed without verification(single keying).
        /// </summary>
        [Description("Displays the field values and allows the value to be appended or changed without verification(single keying).")]
        VisualVerifyAll = 0x00800000,
        /// <summary>
        /// Unflags fields in Review mode when the focus moves to the flagged field.
        /// </summary>
        [Description("Unflags fields in Review mode when the focus moves to the flagged field.")]
        OmniPassAutoUnflag = 0x01000000,
        /// <summary>
        /// Shows the keying window by default in OmniPass mode.
        /// </summary>
        [Description("Shows the keying window by default in OmniPass mode.")]
        ShowKeyingWindow = 0x02000000,
        /// <summary>
        /// Depricated in 2.6
        /// </summary>
        [Browsable(false)]
        PutFlaggedDoc = 0x04000000,
        /// <summary>
        /// Depricated in 2.6
        /// </summary>
        [Browsable(false)]
        OnePassLockFields = 0x08000000,
        /// <summary>
        /// Disables and changes the color of Validated fields in OmniPass unless F2 is pressed.
        /// </summary>
        [Description("Disables and changes the color of Validated fields in OmniPass unless F2 is pressed.")]
        OmniPassToggleSkip = 0x10000000,
        /// <summary>
        /// Displays all defined and OCR zones in the keying window.
        /// </summary>
        [Description("Displays all defined and OCR zones in the keying window.")]
        KeyingWindowFullZone = 0x20000000,
        /// <summary>
        /// Blanks the textbox when error occurs in the OnValidate event.
        /// </summary>
        [Description("Blanks the textbox during error in the OnValidate event.")]
        BlankOnValidate = 0x40000000,
        /// <summary>
        /// Update field status to 'Keyed' instead of 'Verified' during field correction in Omnipass mode.
        /// </summary>
        [Description("Update field status to 'Keyed' instead of 'Verified' during field correction in Omnipass mode")]
        OmniPassFieldCorrectKeyed = 0x80000000,
        /// <summary>
        /// Scale the overlay with the image.
        /// </summary>
        [Description("Scale the overlay to fit the document image.")]
        ScaleOverlay = 0x100000000,
        /// <summary>
        /// During unreject, document resumes from the rejected field.
        /// </summary>
        [Description("During unreject, document resumes from the rejected field.")]
        RejectResume = 0x200000000,
        /// <summary>
        /// Validates the entire form to include all field changes before leaving the form.
        /// </summary>
        [Description("Validates the entire form to include all field changes before leaving the form.")]
        ReValidateForm = 0x400000000,
        /// <summary>
        /// Allows annotating the image in WebDX during data entry.
        /// </summary>
        [Description("Allows annotating the image in WebDX during data entry.")]
        AllowAnnotation = 0x800000000,
        /// <summary>
        /// Splits document before the current selected image in indexing mode. By default it splits after the selected image.
        /// </summary>
        [Description("Splits document before the selected image in indexing mode. By default it splits after the selected image.")]
        DocSplitBeforeCurrentImage = 0x1000000000,
        /// <summary>
        /// Increases the thumbnail window view by doubling its default size in WebDX client.
        /// </summary>
        [Description("Increases the thumbnail window view by doubling its default size in WebDX client.")]
        EnlargeThumbnailView = 0x2000000000,
        /// <summary>
        /// Navigates to the field linked to the current zone. Default is true.
        /// </summary>
        [Description("Navigates to the field linked to the current zone. Default is true.")]
        ZoneDoubleClick = 0x4000000000        
    };

	/// <summary>
	/// The type of error an edit failure causes.
	/// </summary>
	public enum WDEEditErrorType {
		/// <summary>
		/// Data entry cannot continue until the edit passes.
		/// </summary>
		Failure,
		/// <summary>
		/// Show a message indicating an error occurred, but allow processing to continue.
		/// </summary>
		Warning,
		/// <summary>
		/// Show a message and stop processing to allow the situation to be corrected. If it cannot be corrected, continue.
		/// </summary>
		WarningWithRetry,
		/// <summary>
		/// Ingore the error and continue.
		/// </summary>
		Ignore
	};

	/// <summary>
	/// The ocr repair type grouping
	/// </summary>
	public enum WDEOCRRepairMode {
		/// <summary>
		/// No OCR character repair performed.
		/// </summary>
		None,
		/// <summary>
		/// Perform OCR character repair during AlphaNumeric repair mode.
		/// </summary>
		AlphaNumeric,
		/// <summary>
		/// Perform OCR character repair during Numeric repair mode.
		/// </summary>
		Numeric,
		/// <summary>
		/// Perform OCR character repair during Alpha only repair mode.
		/// </summary>
		Alpha
	};

	/// <summary>
	/// Check digit verification methods
	/// </summary>
	[Flags]
	public enum WDECheckDigitMethods 
	{
		/// <summary>
		/// No check digit
		/// </summary>
		None = 0,
		/// <summary>
		/// Mod10 method
		/// </summary>
		Mod10 = 1,
		/// <summary>
		/// Visa credit card
		/// </summary>
		Visa = 4,
		/// <summary>
		/// MasterCard credit card
		/// </summary>
		MasterCard = 8,
		/// <summary>
		/// AMEX credit card
		/// </summary>
		AMEX = 16,
		/// <summary>
		/// Discover credit card
		/// </summary>
		Discover = 32,
		/// <summary>
		/// Diners Club credit card
		/// </summary>
		Diners = 64,
		/// <summary>
		/// ISBN method
		/// </summary>
		ISBN = 128,
		/// <summary>
		/// UPC method
		/// </summary>
		UPC = 512,
		/// <summary>
		/// UPS Tracking number method
		/// </summary>
		UPSTracking = 1024,
		/// <summary>
		/// UPS PBN number method
		/// </summary>
		UPSPBN = 2048
	};

	/// <summary>
	/// Project-wide options
	/// </summary>
	[Flags]
	public enum WDEProjectOption
	{
		/// <summary>
		/// No options
		/// </summary>
		None = 0,
		/// <summary>
		/// Track the image on a per-field basis.
		/// </summary>
        [Description("If true, the current image is kept track of during keying and displayed again in subsequent visits to the same field.")]
		TrackImage = 1,
		/// <summary>
		/// Shows an error message if an invalid character is typed during character repair. If not set, only an audible beep is generated.
		/// </summary>
        [Browsable(false)]
		ShowCharSetError = 2,
        /// <summary>
        /// Track the detail grid row deletion.
        /// </summary>
        [Description("If true, the system will track the row deletion by assigning blank value to all the fields in the detail grid row.")]
        TrackRowDeletion = 4,
        /// <summary>
        /// Sort reject codes numerically.
        /// </summary>
        [Description("If true, the system will sort the reject codes numerically in WebDXClient.")]
        NumericRejectCodes = 8,
        /// <summary>
        /// Display details on Put Work.
        /// </summary>
        [Description("If true, the system will display the details of the work that is put after completion.")]
        DetailsOnPutWork = 16,
        /// <summary>
        /// Display details of Get Work.
        /// </summary>
        [Description("If true, the system will display the details of the work that is pulled for data entry.")]
        DetailsOfGetWork = 32,
        /// <summary>
        /// Destructive Enter.
        /// </summary>
        [Description("If true, clears the field data from the current cursor position when ENTER is hit.")]
        DestructiveEnter = 64,
	};

	/// <summary>
	/// Default image scaling
	/// </summary>
	public enum WDEImageScale {
		/// <summary>
		/// Fit the width to the screen area.
		/// </summary>
        [Description("Fit the width to the screen area.")]
		FitWidth,
		/// <summary>
		/// Fit the height to the screen area.
		/// </summary>
        [Description("Fit the height to the screen area.")]
		FitHeight,
		/// <summary>
		/// Show the entire image using either FitWidth or FitHeight, whichever is better.
		/// </summary>
        [Description("Show the entier image using either FitWidth or FitHeight, whichever is better.")]
		FitBest,
		/// <summary>
		/// Scale to a specific percentage.
		/// </summary>
        [Description("Scale the image to a specific percentage.")]
		ScalePercent
	};

	/// <summary>
	/// Horizontal or vertical split of image and data form.
	/// </summary>
	public enum WDESessionStyle {
		/// <summary>
		/// Split the keying area horizontally.
		/// </summary>
		Horizontal,
		/// <summary>
		/// Split the keying area vertically.
		/// </summary>
		Vertical
	};

	/// <summary>
	/// Form control options.
	/// </summary>
	[Flags]
	public enum WDEControlOption
	{
		/// <summary>
		/// No options
		/// </summary>
		None = 0,
		/// <summary>
		/// Exclude this control from the FieldRepair pass.
		/// </summary>
        [Browsable(false)]
		ExcludeFromFieldRepair = 1,
		/// <summary>
		/// Skip this control during Verify.
		/// </summary>
        [Description("Skip this text box during Verify.")]        
		SkipVerify = 2,
		/// <summary>
		/// Skip this control during QIVerify.
		/// </summary>
        [Description("Skip this text box during QI Verify.")]
		SkipQIVerify = 4,
		/// <summary>
		/// Verify this control at the control level rather than the field level.
		/// </summary>
        [Description("If true, each keystroke will not be verified during verify mode, but verification will happen only when the field is about to be exited.")]
		FieldVerify = 8,
		/// <summary>
		/// This control behaves as if it were in Entry mode when in Verify mode.
		/// </summary>
        [Description("If true, the text box will behave the same in verify mode as it does in entry mode.")]
		EntryModeInVerify = 16,
		/// <summary>
		/// Show the entire image rather than a zoomed in view when this control is active.
		/// </summary>
        [Browsable(false)]
		ShowEntireImage = 32,
		/// <summary>
		/// Show a snippet view rather than a zoomed in view when this control is active.
		/// </summary>
        [Browsable(false)]
		ShowSnippetImage = 64,
		/// <summary>
		/// Allow the control to be flagged.
		/// </summary>
        [Description("Allow the control to be flagged.")]
		AllowFlag = 128,
		/// <summary>
		/// When the maximum data length is reached, automatically advance to the next control.
		/// </summary>
        [Description("If true, when the maximum data length is reached, automatically advance to the next control.")]
		AutoAdvance = 256,
		/// <summary>
		/// Force the overlay to be shown initially when this control is active.
		/// </summary>
        [Browsable(false)]
		ShowOverlay = 512,
        /// <summary>
        /// The field is mostly numeric when keying on an emulated keyboard.
        /// </summary>
        [Description("When keying using an emulated keyboard mode such as embedded 10-key or embedded 029: If true, the text box will key numeric characters from the embedded numeric keys and Shift must be used to produce letters. If false, the embedded numeric keys will produce letters by default and shift must be used to produce numeric characters.")]
        NumericShift = 1024,
        /// <summary>
        /// The field data must always match the maximum field length if the field contains data.
        /// </summary>
        [Description("The entire length of data must be keyed if data is keyed.")]
        MustComplete = 2048,
        /// <summary>
        /// The field data is always converted to upper case.
        /// </summary>
        [Description("The field data is always upper case.")]
        UpperCase = 4096,        
        /// <summary>
        /// The field data Must be Keyed.
        /// </summary>
        [Description("The data must be keyed.")]
        MustEnter = 8192,
        /// <summary>
        /// Selects the imagetype linked to the field zone.
        /// </summary>
        [Description("Selects the imagetype linked to the field's zone when the current imagetype has no zone.")]
        SwitchNoZoneImage = 16384
	};

	/// <summary>
	/// Field options.
	/// </summary>
	[Flags]
	public enum WDEFieldOption
	{
		/// <summary>
		/// No option.
		/// </summary>
		None = 0,
		/// <summary>
		/// Allow the field to be flagged.
		/// </summary>
		AllowFlag = 1,
		/// <summary>
		/// The field data must always match the maximum field length if the field contains data.
		/// </summary>
		MustComplete = 2,
		/// <summary>
		/// The field data is always converted to upper case.
		/// </summary>
		UpperCase = 4,
		/// <summary>
		/// The field is mostly numeric when keying on an emulated keyboard.
		/// </summary>
		NumericShift = 8,        
        /// <summary>
        /// The field data Must be Keyed.
        /// </summary>
        MustEnter = 16
	};

	/// <summary>
	/// GroupBox character repair options
	/// </summary>
	[Flags]
	public enum WDEGroupBoxOption
	{
		/// <summary>
		/// No options.
		/// </summary>
		None = 0,
		/// <summary>
		/// Repair all controls as a group.
		/// </summary>
		RepairEntireGroup = 1,
		/// <summary>
		/// Clear all controls before repairing.
		/// </summary>
		ClearDuringRepair = 2
	};

	/// <summary>
	/// Photostitch orientation
	/// </summary>
	public enum WDEPSOrientation {
		/// <summary>
		/// Horizontal layout.
		/// </summary>
		Horizontal,
		/// <summary>
		/// Vertical layout.
		/// </summary>
		Vertical
	};

	/// <summary>
	/// Define when an edit executes.
	/// </summary>
	public enum WDEExecuteOption {
		/// <summary>
		/// Execute during the validation phase.
		/// </summary>
		Validate,
		/// <summary>
		/// Execute during the control exit phase whether going forward or backward.
		/// </summary>
		ExitAlways,
		/// <summary>
		/// Execute during the control exit phase only when going forward.
		/// </summary>
		ExitForward
	};

	/// <summary>
	/// Table lookup options.
	/// </summary>
	[Flags]
	public enum WDETableLookupOption
	{
		/// <summary>
		/// No options.
		/// </summary>
		None = 0,
		/// <summary>
		/// Cause an edit error if there are no resulting records.
		/// </summary>
		FailIfNoRecords = 1,
		/// <summary>
		/// Stop at each plugged field to review the plug results.
		/// </summary>
		ReviewResults = 2,
		/// <summary>
		/// Allow filtering in the results window.
		/// </summary>
		Filter = 4,
		/// <summary>
		/// Show the results window even if there is only one result record.
		/// </summary>
		OneHitPopup = 8,
		/// <summary>
		/// Convert database nulls to spaces before plugging values.
		/// </summary>
		NullsToSpaces = 16,
		/// <summary>
		/// Perform the lookup even if the field is blank.
		/// </summary>
		LookupIfBlank = 32,
		/// <summary>
		/// Save the position of the results window for future use.
		/// </summary>
		SavePos = 64,
		/// <summary>
		/// Highlight field value differences in the results window.
		/// </summary>
		ShowDiffs = 128,
		/// <summary>
		/// Force verification of plugged data during Verify mode.
		/// </summary>
		VerifyPluggedData = 256
	};

	/// <summary>
	/// Result field options for the results window.
	/// </summary>
	[Flags]
	public enum WDELookupResultFieldOption
	{
		/// <summary>
		/// No options.
		/// </summary>
		None = 0,
		/// <summary>
		/// Display this field in the results window.
		/// </summary>
		Display = 1,
		/// <summary>
		/// Allow filtering on this field in the results window.
		/// </summary>
		AllowFilter = 2
	};

	/// <summary>
	/// Detail grid options
	/// </summary>
	[Flags]
	public enum WDEDetailGridOption
	{
		/// <summary>
		/// No options.
		/// </summary>
		None = 0,
		/// <summary>
		/// The grid can only be exited on the first field in the record.
		/// </summary>
        [Description("If true, the detail grid can only be exited when the first field is focused. If false, the detail grid can be exited on any text box.")]
		RestrictExit = 1,
		/// <summary>
		/// Do not show the header.
		/// </summary>
        [Description("If true, the header will not be shown.")]
		HideHeader = 2,
        /// <summary>
        /// Key vertically down columns.
        /// </summary>
        [Description("If true, keying will take place down columns instead of across rows.")]
        VerticalKey = 4,
        /// <summary>
        /// Identifies the grid as a Columnar layout grid for WebDX Studio.
        /// </summary>
        [Browsable(false)]
        ColumnarLayout = 8,
        /// <summary>
        /// Allows option to map detailgrid row(s) to document image(s).
        /// </summary>
        [Description("If true, maps detailgrid row(s) to the document image(s).")]
        ImageData = 16
	};

	/// <summary>
	/// Record number postion in a detail grid
	/// </summary>
	public enum WDERecordNumberPosition	{
		/// <summary>
		/// Do not show the record number.
		/// </summary>
        [Description("Hide the record number watermark.")]
		Invisible,
		/// <summary>
		/// Show the record number on the left.
		/// </summary>
        [Description("Show the record number watermark on the left side of the detail grid.")]
		Left,
		/// <summary>
		/// Show the record number on the right.
		/// </summary>
        [Description("Show the record number watermark on the right side of the detail grid.")]
		Right,
        /// <summary>
        /// Show the record number on both sides (left and right).
        /// </summary>
        [Description("Show the record number watermark on left and right side of the detail grid.")]
        Both
	};

	/// <summary>
	/// QI options for fields.
	/// </summary>
	public enum WDEQIOption {
		/// <summary>
		/// The field is a critical quality field.
		/// </summary>
        [Description("This field is a critical quality field.")]
		Critical,
		/// <summary>
		/// The field is not a critical quality field.
		/// </summary>
        [Description("This field is not a critical quality field.")]
		NonCritical,
		/// <summary>
		/// The field should be ignored by the quality process.
		/// </summary>
        [Description("This field should be ignored by the quality process.")]
		Exclude
	};

	/// <summary>
	/// Zip lookup edit options.
	/// </summary>
	[Flags]
	public enum WDEZipLookupOption
	{
		/// <summary>
		/// No options.
		/// </summary>
		None = 0,
		/// <summary>
		/// Override the city code using pre-defined defaults to aid the lookup process.
		/// </summary>
		CityCodeOverride = 1,
		/// <summary>
		/// Always show the results window even if there is only one result.
		/// </summary>
		OneHitPopup = 2,
		/// <summary>
		/// Stop at each field to review the results after plugging data.
		/// </summary>
		ReviewResults = 4
	};

    /// <summary>
    /// Textbox error override types
    /// </summary>
    public enum WDEErrorOverrideType
    {
        /// <summary>
        /// Must Complete error override.
        /// </summary>
        MustComplete,
        /// <summary>
        /// Must Enter error override.
        /// </summary>
        MustEnter,
        /// <summary>
        /// CharSet error override.
        /// </summary>
        CharSet,
        /// <summary>
        /// InputMask error override.
        /// </summary>
        InputMask,
        /// <summary>
        /// Default, displays built-in error.
        /// </summary>
        None
    };

    /// <summary>
    /// Image dock modes for the WebDXClient
    /// </summary>
    public enum ImageDock
    {
        /// <summary>
        /// Docks control to the left
        /// </summary>
        Left = 0,
        /// <summary>
        /// Docks control to the right
        /// </summary>
        Right = 1, 
        /// <summary>
        /// Docks control to the top
        /// </summary>
        Top = 2,        
        /// <summary>
        /// Docks control to the top
        /// </summary>
        [Browsable(false)]
        Float = 4
    };

    /// <summary>
    /// Toolbar dock modes for the WebDXClient
    /// </summary>
    public enum ToolbarDock
    {
        /// <summary>
        /// Docks control to the left
        /// </summary>
        Left=0, 
        /// <summary>
        /// Docks control to the right
        /// </summary>
        Right=1,
        /// <summary>
        /// Docks control to the top
        /// </summary>
        Top=2
    };

	/// <summary>
	/// Manages WebDX Project files.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("19DD2094-FB8F-4732-BF7A-802367CB2E5B")]
	public interface IWDEProject
	{
		/// <summary>
		/// Gets the current version of the API.
		/// </summary>
		string APIVersion {get;}
		/// <summary>
		/// Gets the current version of the API in a user friendly format.
		/// </summary>
		string About {get; set;}
		/// <summary>
		/// Gets or sets the creator of this project.
		/// </summary>
		string CreatedBy {get; set;}
		/// <summary>
		/// Gets or sets the time the project was created.
		/// </summary>
		DateTime DateCreated {get; set;}
		/// <summary>
		/// Gets or sets the time the project was last saved.
		/// </summary>
		DateTime DateModified {get; set;}
		/// <summary>
		/// Gets or sets the last modifier of this project.
		/// </summary>
		string ModifiedBy {get; set;}
		/// <summary>
		/// Gets or sets the project description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets the Documents collection.
		/// </summary>
		IWDEDocumentDefs DocumentDefs {get;}
		/// <summary>
		/// Gets or sets the project options.
		/// </summary>
		WDEProjectOption Options {get; set;}
		/// <summary>
		/// Gets or sets the script language. Script languages are read only once document elements are defined.
		/// </summary>
		WDEScriptLanguage ScriptLanguage {get; set;}
		/// <summary>
		/// Gets or sets the current script text for the project.
		/// </summary>
		string Script {get; set;}
		/// <summary>
		/// Gets the Session Defs collection.
		/// </summary>
		IWDESessionDefs SessionDefs {get;}
		/// <summary>
		/// Gets the project default colors.
		/// </summary>
		IWDEProjectColors ProjectColors {get;}
		/// <summary>
		/// Gets the OnDocumentRejected event definition.
		/// </summary>
		IWDEEventScriptDef OnDocumentRejected {get;}
		/// <summary>
		/// Gets the OnDocumentUnrejected event definition.
		/// </summary>
		IWDEEventScriptDef OnDocumentUnRejected {get;}
		/// <summary>
		/// Gets or sets a user defined version string
		/// </summary>
		string Version {get; set;}
		/// <summary>
		/// Gets a collection of database connection strings for this project.
		/// </summary>
		IWDEDatabases Databases {get;}

		/// <summary>
		/// Loads the project from the given stream.
		/// </summary>
		/// <param name="aStream">The System.IO.Stream to load from.</param>
		void LoadFromStream(System.IO.Stream aStream);
		/// <summary>
		/// Loads the project from the given stream.
		/// </summary>
		/// <param name="aStream">The COM stream to load from. Used with COM Interop.</param>
		void LoadFromStream(System.Runtime.InteropServices.ComTypes.IStream aStream);
		/// <summary>
		/// Loads the project from the given file name.
		/// </summary>
		/// <param name="FileName">The fill path and file name to load from.</param>
		void LoadFromFile(string FileName);
		/// <summary>
		/// Loads the project from the given byte array.
		/// </summary>
		/// <param name="bytes">The byte array to load from.</param>
		void LoadFromBytes(byte[] bytes);
		/// <summary>
		/// Clears all project data.
		/// </summary>
		void Clear();
	}

    /// <summary>
    /// Manages WebDX Project Files
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("BFC6B14F-D72F-4d5d-A70D-2D78215B4262")]
    public interface IWDEProject_R1 : IWDEProject
    {
        /// <summary>
        /// Gets the OnKeyPress event.
        /// </summary>
        IWDEEventScriptDef OnKeyPress { get;}
        /// <summary>
        /// Gets the OnStartWork event.
        /// </summary>
        IWDEEventScriptDef OnStartWork { get;}
        /// <summary>
        /// Gets the OnEndWork event.
        /// </summary>
        IWDEEventScriptDef OnEndWork { get;}
        /// <summary>
        /// Gets the OnPageChange event.
        /// </summary>
        IWDEEventScriptDef OnPageChange { get;}
        /// <summary>
        /// Gets a list of external assemblies that implement script plugins for this project.
        /// </summary>
        List<string> ExternalAssemblies { get;}
        /// <summary>
        /// Gets a list of referenced assemblies for use with compiling and running script plugins.
        /// </summary>
        List<string> References { get;}
    }

    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("35CCE045-C82E-4b4f-89BF-C70948EB7847")]
    public interface IWDEProject_R2 : IWDEProject_R1
    {
        /// <summary>
        /// Gets or sets the image scrolling interval for ALT+Arrow keys
        /// </summary>
        int ImageScrollInterval { get; set; }
    }

    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("10A0E9E6-3939-42D4-B367-CCE2CD366AE1")]
    public interface IWDEProject_R3 : IWDEProject_R2
    {
        /// <summary>
        /// Gets or sets the image scrolling interval for ALT+Arrow keys
        /// </summary>
        Color SelectedForeColor { get; set; }
        /// <summary>
        /// Gets or sets the image scrolling interval for ALT+Arrow keys
        /// </summary>
        Color SelectedBackground { get; set; }
    }

    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("A1C08446-44D4-4408-B6FF-844262B055AB")]
    public interface IWDEProject_R4 : IWDEProject_R3
    {
        /// <summary>
        /// Gets or sets the hint bar font.
        /// </summary>
        Font HintFont { get; set; }
    }

    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("A5B063A6-73B1-4CBC-9B7E-C04B43DD8DAB")]
    public interface IWDEProject_R5 : IWDEProject_R4
    {
        /// <summary>
        /// Gets or sets the key board modes.
        /// </summary>
        WDEKeyMode KeyboardModes { get; set; }
    }

	/// <summary>
	/// The project databases collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("8C909CE4-1DCB-4451-90D4-2D4455E48352")]
	public interface IWDEDatabases
	{
		/// <summary>
		/// Gets the number of databases in this colleciton.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets a database by name.
		/// </summary>
		IWDEDatabase this[string Name] {get;}
		/// <summary>
		/// Gets a database by index.
		/// </summary>
		IWDEDatabase this[int Index] {get;}
	}

	/// <summary>
	/// A database definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("CB0A4903-5B19-481a-99A9-FDFB1B6CEB93")]
	public interface IWDEDatabase
	{
		/// <summary>
		/// Gets the database name.
		/// </summary>
		string Name {get;}
		/// <summary>
		/// Gets the connection string.
		/// </summary>
		string ConnectionString {get;}
		/// <summary>
		/// Gets a value indicating whether the database is downloaded or accessed remotely.
		/// </summary>
		bool Download {get;}
	}

	/// <summary>
	/// The default colors for a project.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("C7969771-105E-4adf-808F-0E5AD7C19746")]
	public interface IWDEProjectColors
	{
		/// <summary>
		/// Gets or sets the photostitch cell border color.
		/// </summary>
		Color CellBorderColor {get; set;}
		/// <summary>
		/// Gets or sets the photostitch cell background color.
		/// </summary>
		Color CellColor {get; set;}
		/// <summary>
		/// Gets or sets the snippet border color.
		/// </summary>
		Color SnippetBorderColor {get; set;}
		/// <summary>
		/// Gets or sets the snippet background color.
		/// </summary>
		Color SnippetColor {get; set;}
		/// <summary>
		/// Gets or sets the zone border color.
		/// </summary>
		Color ZoneBorderColor {get; set;}
		/// <summary>
		/// Gets or sets the zone background color.
		/// </summary>
		Color ZoneColor {get; set;}
		/// <summary>
		/// Gets or sets the selected color.
		/// </summary>
		Color SelectedColor {get; set;}
		/// <summary>
		/// Gets or sets the excluded color.
		/// </summary>
		Color ExcludedColor {get; set;}
		/// <summary>
		/// Resets colors to default values.
		/// </summary>
		void SetDefaults();
	}

	/// <summary>
	/// Project Manager interface.
	/// </summary>
	[ComVisible(false)]
	public interface IWDEProjectPM
	{
		/// <summary>
		/// Saves the project to the given stream.
		/// </summary>
		/// <param name="aStream">The System.IO.Stream object to save to.</param>
		void SaveToStream(System.IO.Stream aStream);
		/// <summary>
		/// Saves the project to the given stream.
		/// </summary>
		/// <param name="aStream">The COM stream to save to.</param>
		void SaveToStream(System.Runtime.InteropServices.ComTypes.IStream aStream);
		/// <summary>
		/// Saves the project to the given file name.
		/// </summary>
		/// <param name="FileName">The full path and file name to save to.</param>
		void SaveToFile(string FileName);
	}

#if DEBUG
	public interface IWDEProjectInternal
#else
    internal interface IWDEProjectInternal
#endif
	{
		LinkResolver Resolver {get; set;}
		bool ConvertOldFormat {get; set;}
		void AppendOldScriptText(string ScriptText, string NamePath);
		void AppendOldExpression(string Expression, string NamePath);
		ArrayList GetTopLevelCollections();
	}

	internal interface IWDELinkConverter
	{
		object ConvertLinkObject(object linkTarget);
	}

	/// <summary>
	/// Document definition collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("6E3A54B2-8A32-4417-8F63-5F327D03F20A")]
	public interface IWDEDocumentDefs : IEnumerable
	{
		/// <summary>
		/// Gets the number of document defs in this collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the document def at the given index.
		/// </summary>
		IWDEDocumentDef this [int Index] {get;}
		/// <summary>
		/// Find a document def by doctype.
		/// </summary>
		/// <param name="DocType">The DocType to search for.</param>
		/// <returns>The index of the matching document def. Returns -1 if no match is found.</returns>
		int Find(string DocType);
		/// <summary>
		/// Add a new document def for a given DocType.
		/// </summary>
		/// <param name="DocType">The DocType to add.</param>
		/// <returns>The newly added document def.</returns>
		IWDEDocumentDef Add(string DocType);
		/// <summary>
		/// Add a new document def using the default DocType naming.
		/// </summary>
		/// <returns>The newly added document def.</returns>
		IWDEDocumentDef Add();
		/// <summary>
		/// Removes all document defs from the collection.
		/// </summary>
		void Clear();
	}

	/// <summary>
	/// A document definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("862E2863-AD3F-44f9-88BC-2B75BA2AC8F5")]
	public interface IWDEDocumentDef
	{
		/// <summary>
		/// Gets or sets description of this document def.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the DocType for this document def.
		/// </summary>
		string DocType {get; set;}
		/// <summary>
		/// Gets the form defs defined for this document.
		/// </summary>
		IWDEFormDefs FormDefs {get;}
		/// <summary>
		/// Gets the StoredDocType for this document. Used when storing documents in SIR.
		/// </summary>
		string StoredDocType {get; set;}
		/// <summary>
		/// Gets the record defs for this document.
		/// </summary>
		IWDERecordDefs RecordDefs {get;}
		/// <summary>
		/// Gets the image source defs for this document.
		/// </summary>
		IWDEImageSourceDefs ImageSourceDefs {get;}
	}

	/// <summary>
	/// Image definition collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("84982F01-912D-4822-BED6-3A9EA797D979")]
	public interface IWDEImageSourceDefs : IEnumerable
	{
		/// <summary>
		/// Gets the number of image source defs in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the image source def at the given index.
		/// </summary>
		IWDEImageSourceDef this[int index] {get;}
		/// <summary>
		/// Finds an image source def by image type.
		/// </summary>
		/// <param name="ImageType">The image type to find.</param>
		/// <returns>The index of the matching image source def. Returns -1 if there is no match.</returns>
		int Find(string ImageType);
		/// <summary>
		/// Adds an image source of the given type to the collection.
		/// </summary>
		/// <param name="ImageType">The image type to add.</param>
		/// <returns>The newly added image source def.</returns>
		IWDEImageSourceDef Add(string ImageType);
		/// <summary>
		/// Adds an image source of the given type to the collection using default naming.
		/// </summary>
		/// <returns>The newly added image type.</returns>
		IWDEImageSourceDef Add();
		/// <summary>
		/// Removes all image source defs from the collection.
		/// </summary>
		void Clear();
        void Add(IWDEImageSourceDef newItem);
	}

	/// <summary>
	/// Image definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("D01072D5-6D7A-41f2-A69F-E9FF0F83855F")]
	public interface IWDEImageSourceDef
	{
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the image type.
		/// </summary>
		string ImageType {get; set;}
		/// <summary>
		/// Gets or sets the stored attachment type. Used when integrating with SIR.
		/// </summary>
		string StoredAttachType {get; set;}
		/// <summary>
		/// Gets or sets the overlay file name.
		/// </summary>
		string Overlay {get; set;}
		/// <summary>
		/// Gets or sets the template file name. Used in Project Manager.
		/// </summary>
		string Template {get; set;}
		/// <summary>
		/// Obsolete. Provided for backwards compatibility only.
		/// </summary>
		bool PerformOCR {get; set;}
		/// <summary>
		/// Gets the zone defs.
		/// </summary>
		IWDEZoneDefs ZoneDefs {get;}
		/// <summary>
		/// Gets the snippet defs.
		/// </summary>
		IWDESnippetDefs SnippetDefs {get;}
		/// <summary>
		/// Gets the detail zone defs.
		/// </summary>
		IWDEDetailZoneDefs DetailZoneDefs {get;}
        IWDEImageSourceDef Clone();
	}

    /// <summary>
    /// Image definition
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("81507C96-D1D4-4c3f-A671-F2F6DAC4E60C")]
    public interface IWDEImageSourceDef_R1 : IWDEImageSourceDef
    {
        IWDEZoneDef[] GetFlatZoneDefs();
    }

	/// <summary>
	/// Record definition collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("217BF72D-2AE4-4ad3-BC11-7BB988FBF49E")]
	public interface IWDERecordDefs : IEnumerable
	{
		/// <summary>
		/// Gets the number of record defs in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the record def at the given index.
		/// </summary>
		IWDERecordDef this [int index] {get;}
		/// <summary>
		/// Searches for a record def by RecType.
		/// </summary>
		/// <param name="RecType">The type of record to find.</param>
		/// <returns>The index of the matching record def. Returns -1 if not match is found.</returns>
		int Find(string RecType);
		/// <summary>
		/// Create a new record def of the given RecType.
		/// </summary>
		/// <param name="RecType">The type of record to create.</param>
		/// <returns>The newly created record def.</returns>
		IWDERecordDef Add(string RecType);
		/// <summary>
		/// Create a new record def using default naming.
		/// </summary>
		/// <returns>The newly created record def.</returns>
		IWDERecordDef Add();
		/// <summary>
		/// Removes all record defs from the collection.
		/// </summary>
		void Clear();
        /// <summary>
        /// Add a new record def.
        /// </summary>
        /// <param name="def">The type of record to add.</param>
        void Add(IWDERecordDef def);
	}

	/// <summary>
	/// A record definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("68867F10-0275-41bc-9415-346B4AE71193")]
	public interface IWDERecordDef
	{
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the design rect for this record def. Used in Project Manager.
		/// </summary>
		Rectangle DesignRect {get; set;}
		/// <summary>
		/// Gets the parent document.
		/// </summary>
		IWDEDocumentDef Document {get;}
		/// <summary>
		/// Gets the field defs.
		/// </summary>
		IWDEFieldDefs FieldDefs {get;}
		/// <summary>
		/// Gets or sets the RecType.
		/// </summary>
		string RecType {get; set;}
		/// <summary>
		/// Gets or sets the max records for this rec type. Zero means no maximum.
		/// </summary>
		int MaxRecs {get; set;}
		/// <summary>
		/// Gets or sets the min records for this rec type. Zero means no minimum.
		/// </summary>
		int MinRecs {get; set;}
		/// <summary>
		/// Gets the child record defs.
		/// </summary>
		IWDERecordDefs RecordDefs {get;}
        IWDERecordDef Clone();
	}

	/// <summary>
	/// A field definition collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("86EE9ECB-F256-4192-ABB3-0E51FFE10032")]
	public interface IWDEFieldDefs : IEnumerable
	{
		/// <summary>
		/// Gets the number of field defs in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the field def at the given index.
		/// </summary>
		IWDEFieldDef this [int index] {get;}
		/// <summary>
		/// Searches for a field def by FieldName.
		/// </summary>
		/// <param name="FieldName">The field name to search for.</param>
		/// <returns>The index of the matching field def. Returns -1 if no match is found.</returns>
		int Find(string FieldName);
		/// <summary>
		/// Searches for a field def by OCRName.
		/// </summary>
		/// <param name="OCRName">The ocr name to search for.</param>
		/// <returns>The index of the matching field def. Returns -1 if no match is found.</returns>
		int FindOCR(string OCRName);
		/// <summary>
		/// Creates a new field def using the given field name.
		/// </summary>
		/// <param name="FieldName">The field name to create.</param>
		/// <returns>The newly created field def.</returns>
		IWDEFieldDef Add(string FieldName);
		/// <summary>
		/// Creates a new field def using default naming.
		/// </summary>
		/// <returns>The newly created field def.</returns>
		IWDEFieldDef Add();
		/// <summary>
		/// Removes all field defs from the collection.
		/// </summary>
		void Clear();
        void Add(IWDEFieldDef def);
	}

	/// <summary>
	/// A field def link collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("5712C0B3-CF88-4a4e-B219-8780E8C0756A")]
	public interface IWDEFieldDefLinks : IEnumerable
	{
		/// <summary>
		/// Gets the number of field defs in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the field def at the given index.
		/// </summary>
		IWDEFieldDef this [int index] {get;}
		/// <summary>
		/// Adds a field def to the collection.
		/// </summary>
		/// <param name="FieldDef">The field def to add.</param>
		void Add(IWDEFieldDef FieldDef);
		/// <summary>
		/// Clears all field defs from the collection.
		/// </summary>
		void Clear();
	}

	/// <summary>
	/// A field definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("B5C0B7E4-80AA-4d4c-9D44-F410936EB82B")]
	public interface IWDEFieldDef
	{
		/// <summary>
		/// Gets or sets an allowed character set definition.
		/// </summary>
		string CharSet {get; set;}
		/// <summary>
		/// Gets or sets the maximum allowed length of data.
		/// </summary>
		int DataLen {get; set;}
		/// <summary>
		/// Obsolete. For backwards compatibility only.
		/// </summary>
		string DataMask {get; set;}
		/// <summary>
		/// Gets or sets the data type. For backwards compatibility only. All fields are currently treated as text fields.
		/// </summary>
		WDEDataType DataType {get; set;}
		/// <summary>
		/// Gets or sets a default value.
		/// </summary>
		string DefaultValue {get; set;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the number of errors allowed before the entire field is rejected in character repair mode.
		/// </summary>
		int OCRAllowedErrors {get; set;}
		/// <summary>
		/// Gets or sets an allowed character set definition that applies to character repair mode.
		/// </summary>
		string OCRCharSet {get; set;}
		/// <summary>
		/// Gets or sets a confidence threshold for character rejects in character repair mode.
		/// </summary>
		int OCRConfidence {get; set;}
		/// <summary>
		/// Gets or sets the pass/mode during which this field should be repaired.
		/// </summary>
		WDEOCRRepairMode OCRRepairMode {get; set;}
		/// <summary>
		/// Gets or sets field options.
		/// </summary>
		WDEFieldOption Options {get; set;}
		/// <summary>
		/// Gets or sets the Central OCR field name.
		/// </summary>
		string OCRName {get; set;}
		/// <summary>
		/// Gets or sets the OCR line number.
		/// </summary>
		int OCRLine {get; set;}
		/// <summary>
		/// Gets or sets a display title.
		/// </summary>
		string FieldTitle {get; set;}
		/// <summary>
		/// Gets the edit defs.
		/// </summary>
		IWDEEditDefs EditDefs {get;}
		/// <summary>
		/// Gets the OnValidate event def.
		/// </summary>
		IWDEEventScriptDef OnValidate {get;}
		/// <summary>
		/// Gets or sets the field name.
		/// </summary>
		string FieldName {get; set;}
		/// <summary>
		/// Gets or sets the QI option.
		/// </summary>
		WDEQIOption QIOption {get; set;}
        IWDEFieldDef Clone();
	}

	/// <summary>
	/// A form def collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("8074DDB1-E146-4f12-BEC1-A40E0357F286")]
	public interface IWDEFormDefs : IEnumerable
	{
		/// <summary>
		/// Gets the number of form defs in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the form def at the given index.
		/// </summary>
		IWDEFormDef this[int index] {get;}
		/// <summary>
		/// Searches for a form by name.
		/// </summary>
		/// <param name="FormName">The form name to search for.</param>
		/// <returns>The index of the matching form def. Returns -1 if no match is found.</returns>
		int Find(string FormName);
		/// <summary>
		/// Creates a new form def using the given form name.
		/// </summary>
		/// <param name="FormName">The form name to create.</param>
		/// <returns>The newly created form def.</returns>
		IWDEFormDef Add(string FormName);
		/// <summary>
		/// Creates a new form using default naming.
		/// </summary>
		/// <returns>The newly created form def.</returns>
		IWDEFormDef Add();
		/// <summary>
		/// Removes all form defs from the collection.
		/// </summary>
		void Clear();
	}

	/// <summary>
	/// A form definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("EC73C55F-CD2F-4209-8911-09C648190EEE")]
	public interface IWDEFormDef
	{
		/// <summary>
		/// Gets or sets the background color.
		/// </summary>
		Color BackColor {get; set;}
		/// <summary>
		/// Gets or sets the foreground color.
		/// </summary>
		Color ForeColor {get; set;}
		/// <summary>
		/// Gets the control def collection.
		/// </summary>
		IWDEControlDefs ControlDefs {get;}
		/// <summary>
		/// Gets the record def that defines the header fields.
		/// </summary>
		IWDERecordDef RecordDef {get; set;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the default font for this form.
		/// </summary>
		Font FormFont {get; set;}
		/// <summary>
		/// Gets or sets the form name.
		/// </summary>
		string FormName {get; set;}
		/// <summary>
		/// Gets or sets a help messsage.
		/// </summary>
		string Help {get; set;}
		/// <summary>
		/// Gets or sets a hint message.
		/// </summary>
		string Hint {get; set;}
		/// <summary>
		/// Gets or sets an indicator to use snippets.
		/// </summary>
		bool UseSnippets {get; set;}
		/// <summary>
		/// Gets or sets the default image.
		/// </summary>
		IWDEImageSourceDef DefaultImage {get; set;}
	}

    /// <summary>
    /// A form definition
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("808E8D5E-593A-4fe8-A791-776D269750FF")]
    public interface IWDEFormDef_R1 : IWDEFormDef
    {
        /// <summary>
        /// Gets the OnEnter definition.
        /// </summary>
        IWDEEventScriptDef OnEnter { get; set; }
        /// <summary>
        /// Gets the OnExit definition.
        /// </summary>
        IWDEEventScriptDef OnExit { get; set; }
    }

	/// <summary>
	/// A visual control collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("B4403608-B96B-437f-BD54-91681CBF200E")]
	public interface IWDEControlDefs : IEnumerable
	{
		/// <summary>
		/// Gets the number of controls in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the control at the given index.
		/// </summary>
		IWDEControlDef this[int index] {get;}
		/// <summary>
		/// Searches for a control by name.
		/// </summary>
		/// <param name="ControlName">The name of the control to find.</param>
		/// <returns>The index of the matching control. Returns -1 if there is no match.</returns>
		int Find(string ControlName);
		/// <summary>
		/// Creates a label using the given name.
		/// </summary>
		/// <param name="Name">The name of the label to create.</param>
		/// <returns>The newly created label.</returns>
		IWDELabelDef AddLabel(string Name);
		/// <summary>
		/// Creates a label using default naming.
		/// </summary>
		/// <returns>The newly created label.</returns>
		IWDELabelDef AddLabel();
		/// <summary>
		/// Creates a text box using the given name.
		/// </summary>
		/// <param name="Name">The name of the text box to create.</param>
		/// <returns>The newly created text box.</returns>
		IWDETextBoxDef AddTextBox(string Name);
		/// <summary>
		/// Creates a text box using default naming.
		/// </summary>
		/// <returns>The newly created text box.</returns>
		IWDETextBoxDef AddTextBox();
		/// <summary>
		/// Creates a group box using the given name.
		/// </summary>
		/// <param name="Name">The name of the group box to create.</param>
		/// <returns>The newly created group box.</returns>
		IWDEGroupBoxDef AddGroupBox(string Name);
		/// <summary>
		/// Creates a group box using default naming.
		/// </summary>
		/// <returns>The newly created group box.</returns>
		IWDEGroupBoxDef AddGroupBox();
		/// <summary>
		/// Creates a detail grid using the given name.
		/// </summary>
		/// <param name="Name">The name of the detail grid to create.</param>
		/// <returns>The newly created detail grid.</returns>
		IWDEDetailGridDef AddDetailGrid(string Name);
		/// <summary>
		/// Creates a detail grid using default naming.
		/// </summary>
		/// <returns>The newly created detail grid.</returns>
		IWDEDetailGridDef AddDetailGrid();
		/// <summary>
		/// Removes all controls from the collection.
		/// </summary>
		void Clear();
		/// <summary>
		/// Gets all controls arranged by key order.
		/// </summary>
		/// <returns>The list of controls in key order.</returns>
		ArrayList GetKeyorder();
        void Add(IWDEControlDef def);
	}

	internal interface IWDEControlDefsInternal
	{
		void MergeKeyOrderList(object subList, int KeyOrderPosition);
		void Add(IWDEControlDef def);
		int GetHighestSuffix(string nameRoot);
        int GetHighestSuffix(string nameRoot, bool examineForm);
	}

	/// <summary>
	/// A collection of control def links.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("87F1A87E-D35A-414d-9834-958FF6717721")]
	public interface IWDEControlDefLinks : IEnumerable
	{
		/// <summary>
		/// Gets the number of control defs in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the control def at the given index.
		/// </summary>
		IWDEControlDef this[int index] {get;}
		/// <summary>
		/// Adds a control def to the collection.
		/// </summary>
		/// <param name="ControlDef">The control def to add.</param>
		void Add(IWDEControlDef ControlDef);
		/// <summary>
		/// Removes all control defs from the collection.
		/// </summary>
		void Clear();
	}

	/// <summary>
	/// The base control interface.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("BFFF08EF-000E-4c4d-BD98-FFD7E036AD29")]
	public interface IWDEControlDef
	{
		/// <summary>
		/// Gets or sets the background color.
		/// </summary>
		Color BackColor {get; set;}
		/// <summary>
		/// Gets or sets the foreground color.
		/// </summary>
		Color ForeColor {get; set;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets help text.
		/// </summary>
		string Help {get; set;}
		/// <summary>
		/// Gets or sets hint text.
		/// </summary>
		string Hint {get; set;}
		/// <summary>
		/// Gets or sets the key order.
		/// </summary>
		int KeyOrder {get; set;}
		/// <summary>
		/// Gets or sets the tab stop flag.
		/// </summary>
		bool TabStop {get; set;}
		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		Rectangle Location {get; set;}
		/// <summary>
		/// Gets or sets the control name.
		/// </summary>
		string ControlName {get; set;}
		/// <summary>
		/// Gets or sets the control font.
		/// </summary>
		Font ControlFont {get; set;}
        IWDEControlDef Clone();
	}

	/// <summary>
	/// A label definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("663BF8B9-1B0D-477a-AE8A-73793B86D3E1")]
	public interface IWDELabelDef : IWDEControlDef
	{
		/// <summary>
		/// Gets or sets the background color.
		/// </summary>
		Color BackColor {get; set;}
		/// <summary>
		/// Gets or sets the foreground color.
		/// </summary>
		Color ForeColor {get; set;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets help text.
		/// </summary>
		string Help {get; set;}
		/// <summary>
		/// Gets or sets hint text.
		/// </summary>
		string Hint {get; set;}
		/// <summary>
		/// Gets or sets the key order.
		/// </summary>
		int KeyOrder {get; set;}
		/// <summary>
		/// Gets or sets the tab stop flag.
		/// </summary>
		bool TabStop {get; set;}
		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		Rectangle Location {get; set;}
		/// <summary>
		/// Gets or sets the control name.
		/// </summary>
		string ControlName {get; set;}
		/// <summary>
		/// Gets or sets the control font.
		/// </summary>
		Font ControlFont {get; set;}
//		/// <summary>
//		/// Gets or sets the text alignment.
//		/// </summary>
//		ContentAlignment Alignment {get; set;}
		/// <summary>
		/// Gets or sets the label text.
		/// </summary>
		string Caption {get; set;}
		/// <summary>
		/// Gets or sets a linked field. The field contents will display as the label text.
		/// </summary>
		IWDEFieldDef Field {get; set;}
		/// <summary>
		/// Gets or sets a linked text box.
		/// </summary>
		IWDEControlDef NextControl {get; set;}
		/// <summary>
		/// Gets or sets the auto size flag. The label size will grow and shrink to fit the contents.
		/// </summary>
		bool AutoSize {get; set;}
	}

	/// <summary>
	/// A text box definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("78314914-EBC1-4d0a-865A-5B7AED8D2E8D")]
	public interface IWDETextBoxDef : IWDEControlDef
	{
		/// <summary>
		/// Gets or sets the background color.
		/// </summary>
		Color BackColor {get; set;}
		/// <summary>
		/// Gets or sets the foreground color.
		/// </summary>
		Color ForeColor {get; set;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}        
		/// <summary>
		/// Gets or sets help text.
		/// </summary>
		string Help {get; set;}
		/// <summary>
		/// Gets or sets hint text.
		/// </summary>
		string Hint {get; set;}
		/// <summary>
		/// Gets or sets the key order.
		/// </summary>
		int KeyOrder {get; set;}
		/// <summary>
		/// Gets or sets the tab stop flag.
		/// </summary>
		bool TabStop {get; set;}
		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		Rectangle Location {get; set;}
		/// <summary>
		/// Gets or sets the control name.
		/// </summary>
		string ControlName {get; set;}
		/// <summary>
		/// Gets or sets the control font.
		/// </summary>
		Font ControlFont {get; set;}
		/// <summary>
		/// Gets or sets an allowed character set definition.
		/// </summary>
		string CharSet {get; set;}
		/// <summary>
		/// Gets the edit def collection.
		/// </summary>
		IWDEEditDefs EditDefs {get;}
		/// <summary>
		/// Gets or sets the linked field def.
		/// </summary>
		IWDEFieldDef Field {get; set;}
		/// <summary>
		/// Gets or sets a display mask.
		/// </summary>
		string InputMask {get; set;}
		/// <summary>
		/// Gets the linked label collection.
		/// </summary>
		IWDELabelLinks LabelLinks {get;}
		/// <summary>
		/// Gets the linked zone collection.
		/// </summary>
		IWDEZoneLinks ZoneLinks {get;}
		/// <summary>
		/// Gets or sets text box options.
		/// </summary>
		WDEControlOption Options {get; set;}
		/// <summary>
		/// Gets the OnValidate event definition.
		/// </summary>
        IWDEEventScriptDef OnValidate { get; set; }
		/// <summary>
		/// Gets the OnEnter event definition.
		/// </summary>
        IWDEEventScriptDef OnEnter { get; set; }
		/// <summary>
		/// Gets the OnExit event definition.
		/// </summary>
        IWDEEventScriptDef OnExit { get; set; }

		/// <summary>
		/// Gets the linked zones by the given image type.
		/// </summary>
		/// <param name="ImageType">The image type to search for.</param>
		/// <returns>The linked zones for the given image type. The array list is empty if there are no matching zones.</returns>
		ArrayList GetZonesByImageType(string ImageType);
		/// <summary>
		/// Gets the snippet for the given zone.
		/// </summary>
		/// <param name="Zone">The zone to find a snippet def for.</param>
		/// <returns>The matching snippet def. Returns null if there is no match.</returns>
		IWDESnippetDef GetSnippet(IWDEZoneDef Zone);
	}

    /// <summary>
    /// A text box definition.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("F7411DED-D632-434e-ABA4-B55EF4194F8E")]
    public interface IWDETextBoxDef_R1 : IWDETextBoxDef
    {
        /// <summary>
        /// Gets the OnKeyPress event definition.
        /// </summary>
        IWDEEventScriptDef OnKeyPress { get; set; }
    }

    /// <summary>
    /// A text box definition.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("F0324926-4631-4F5F-9E57-98B655E0DFBB")]
    public interface IWDETextBoxDef_R2 : IWDETextBoxDef_R1
    {
        /// <summary>
        /// Gets or sets the textbox display name.
        /// </summary>
        string DisplayLabel { get; set; }
    }

    /// <summary>
    /// A text box definition.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("812318A2-1A4F-483D-95CF-68519B953942")]
    public interface IWDETextBoxDef_R3 : IWDETextBoxDef_R2
    {
        /// <summary>
        /// Gets the error overrides collection.
        /// </summary>
        IWDEErrorOverrides ErrorOverrides { get; set; }
    }

	/// <summary>
	/// A group box definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("E8B28D35-79F0-44b4-BBBF-29F1CE02DB7F")]
	public interface IWDEGroupBoxDef : IWDEControlDef
	{
		/// <summary>
		/// Gets or sets the background color.
		/// </summary>
		Color BackColor {get; set;}
		/// <summary>
		/// Gets or sets the foreground color.
		/// </summary>
		Color ForeColor {get; set;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets help text.
		/// </summary>
		string Help {get; set;}
		/// <summary>
		/// Gets or sets hint text.
		/// </summary>
		string Hint {get; set;}
		/// <summary>
		/// Gets or sets the key order.
		/// </summary>
		int KeyOrder {get; set;}
		/// <summary>
		/// Gets or sets the tab stop flag.
		/// </summary>
		bool TabStop {get; set;}
		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		Rectangle Location {get; set;}
		/// <summary>
		/// Gets or sets the control name.
		/// </summary>
		string ControlName {get; set;}
		/// <summary>
		/// Gets or sets the control font.
		/// </summary>
		Font ControlFont {get; set;}
		/// <summary>
		/// Gets or sets the group box text.
		/// </summary>
		string Caption {get; set;}
		/// <summary>
		/// Gets or sets group box options.
		/// </summary>
		WDEGroupBoxOption Options {get; set;}
		/// <summary>
		/// Gets the linked zones.
		/// </summary>
		IWDEZoneLinks ZoneLinks {get;}
		/// <summary>
		/// Gets the contained controls.
		/// </summary>
		IWDEControlDefLinks ControlDefs {get;}
	}

	/// <summary>
	/// A detail grid definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("A0CDA1CE-B501-45eb-B524-CAB7E74D3E57")]
	public interface IWDEDetailGridDef : IWDEControlDef
	{
		/// <summary>
		/// Gets or sets the background color.
		/// </summary>
		Color BackColor {get; set;}
		/// <summary>
		/// Gets or sets the foreground color.
		/// </summary>
		Color ForeColor {get; set;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets help text.
		/// </summary>
		string Help {get; set;}
		/// <summary>
		/// Gets or sets hint text.
		/// </summary>
		string Hint {get; set;}
		/// <summary>
		/// Gets or sets the key order.
		/// </summary>
		int KeyOrder {get; set;}
		/// <summary>
		/// Gets or sets the tab stop flag.
		/// </summary>
		bool TabStop {get; set;}
		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		Rectangle Location {get; set;}
		/// <summary>
		/// Gets or sets the control name.
		/// </summary>
		string ControlName {get; set;}
		/// <summary>
		/// Gets or sets the control font.
		/// </summary>
		Font ControlFont {get; set;}
		/// <summary>
		/// Gets the record def that defines the detail fields.
		/// </summary>
		IWDERecordDef RecordDef {get; set;}
		/// <summary>
		/// Gets the header controls collection.
		/// </summary>
		IWDEControlDefs HeaderControlDefs {get;}
		/// <summary>
		/// Gets or sets the header background color.
		/// </summary>
		Color HeaderBackColor {get; set;}
		/// <summary>
		/// Gets or sets the detail background color.
		/// </summary>
		Color HeaderForeColor {get; set;}
		/// <summary>
		/// Gets or sets the header height.
		/// </summary>
		int HeaderHeight {get; set;}
		/// <summary>
		/// Gets or sets the header font.
		/// </summary>
		Font HeaderFont {get; set;}
		/// <summary>
		/// Gets or sets detail grid options.
		/// </summary>
		WDEDetailGridOption Options {get; set;}
		/// <summary>
		/// Gets or sets the number of visible rows in the grid.
		/// </summary>
		int Rows {get; set;}
		/// <summary>
		/// Gets or sets the position of the record number display.
		/// </summary>
		WDERecordNumberPosition RecordNumberPosition {get; set;}
		/// <summary>
		/// Gets or sets the linked detail zone def.
		/// </summary>
		IWDEDetailZoneDef DetailZoneDef {get; set;}
		/// <summary>
		/// Gets the OnEnter event definition.
		/// </summary>
		IWDEEventScriptDef OnEnter {get;}
		/// <summary>
		/// Gets the OnExit event definition.
		/// </summary>
		IWDEEventScriptDef OnExit {get;}
		/// <summary>
		/// Gets the OnValidate event definition.
		/// </summary>
		IWDEEventScriptDef OnValidate {get;}
		/// <summary>
		/// Gets the child control def collection.
		/// </summary>
        IWDEControlDefs ControlDefs { get; set; }
	}

    /// <summary>
    /// A form definition.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("7BECD4EF-10D5-4D86-9F64-6AAC92F0D44B")]
    public interface IWDEImageFormDef
    {
        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        Color BackColor { get; set; }
        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        Color ForeColor { get; set; }
        /// <summary>
        /// Gets the control def collection.
        /// </summary>
        IWDEControlDefs ControlDefs { get; }
        /// <summary>
        /// Gets the record def that defines the header fields.
        /// </summary>
        IWDERecordDef RecordDef { get; set; }
        /// <summary>
        /// Gets or sets the default font for this form.
        /// </summary>
        Font FormFont { get; set; }
        /// <summary>
        /// Gets the image source defs for this document.
        /// </summary>
        IWDEImageSourceDef ImageSourceDef { get; set; }
        /// <summary>
        /// Gets the OnEnter definition.
        /// </summary>
        IWDEEventScriptDef OnEnter { get; set; }
        /// <summary>
        /// Gets the OnExit definition.
        /// </summary>
        IWDEEventScriptDef OnExit { get; set; }
    }

    /// <summary>
    /// An image form def collection.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("B598C565-A853-433D-BDE6-7BCEC68F6FE7")]
    public interface IWDEImageFormDefs : IEnumerable
    {
        /// <summary>
        /// Gets the number of form defs in the collection.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets the form def at the given index.
        /// </summary>
        IWDEImageFormDef this[int index] { get; }
        /// <summary>
        /// Searches for a form by image type.
        /// </summary>
        /// <param name="ImageType">The image type to search for.</param>
        /// <returns>The index of the matching form def. Returns -1 if no match is found.</returns>
        int Find(string ImageType);
        /// <summary>
        /// Creates a new form def using the given image type.
        /// </summary>
        /// <param name="ImageType">The image type to name the form.</param>
        /// <returns>The newly created form def.</returns>
        IWDEImageFormDef Add(string ImageType);
        /// <summary>
        /// Removes all form defs from the collection.
        /// </summary>
        void Clear();
    }

	/// <summary>
	/// A image data definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("8E3646E7-BD39-4C51-B6E9-CF9B785D4BA0")]
	public interface IWDEImageDataDef : IWDEControlDef
	{
		/// <summary>
		/// Gets or sets the background color.
		/// </summary>
		Color BackColor {get; set;}
		/// <summary>
		/// Gets or sets the foreground color.
		/// </summary>
		Color ForeColor {get; set;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets help text.
		/// </summary>
		string Help {get; set;}
		/// <summary>
		/// Gets or sets hint text.
		/// </summary>
		string Hint {get; set;}
		/// <summary>
		/// Gets or sets the key order.
		/// </summary>
		int KeyOrder {get; set;}
		/// <summary>
		/// Gets or sets the tab stop flag.
		/// </summary>
		bool TabStop {get; set;}
		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		Rectangle Location {get; set;}
		/// <summary>
		/// Gets or sets the control name.
		/// </summary>
		string ControlName {get; set;}
		/// <summary>
		/// Gets or sets the control font.
		/// </summary>
		Font ControlFont {get; set;}
		/// <summary>
		/// Gets or sets the header height.
		/// </summary>
		int HeaderHeight {get; set;}
		/// <summary>
		/// Gets or sets the header font.
		/// </summary>
		Font HeaderFont {get; set;}
		/// <summary>
		/// Gets the OnEnter event definition.
		/// </summary>
		IWDEEventScriptDef OnEnter {get;}
		/// <summary>
		/// Gets the OnExit event definition.
		/// </summary>
		IWDEEventScriptDef OnExit {get;}
		/// <summary>
		/// Gets the child control def collection.
		/// </summary>
        IWDEImageFormDefs FormDefs { get; set; }
	}

	/// <summary>
	/// A zone link collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("0ECF0B60-1AE9-4b7b-A890-450A0A060B9F")]
	public interface IWDEZoneLinks : IEnumerable
	{
		/// <summary>
		/// Gets the number of zones in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the zone at the given index.
		/// </summary>
		IWDEBaseZoneDef this[int index] {get;}
		/// <summary>
		/// Adds a zone def to the collection.
		/// </summary>
		/// <param name="ZoneDef">The zone def to add.</param>
		void Add(IWDEBaseZoneDef ZoneDef);
		/// <summary>
		/// Removes all zones from the collection.
		/// </summary>
		void Clear();
	}

	/// <summary>
	/// A label link collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("A0F26E8E-3C02-4844-8668-FDC11DC67FED")]
	public interface IWDELabelLinks : IEnumerable
	{
		/// <summary>
		/// Gets the number of label defs in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the label def at the given index.
		/// </summary>
		IWDELabelDef this[int index] {get;}
		/// <summary>
		/// Adds a label def to the collection.
		/// </summary>
		/// <param name="LabelDef">The label def to add.</param>
		void Add(IWDELabelDef LabelDef);
		/// <summary>
		/// Removes all label defs from the collection.
		/// </summary>
		void Clear();
	}

	/// <summary>
	/// Photostitch settings definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("52AFE26A-9002-49aa-B5F9-A97091D2EEE8")]
	public interface IWDEPhotoStitchDef
	{
		/// <summary>
		/// Gets or sets the number of columns in the image grid.
		/// </summary>
		int ColCount {get; set;}
		/// <summary>
		/// Gets or sets the number of rows in the image grid.
		/// </summary>
		int RowCount {get; set;}
		/// <summary>
		/// Gets or sets the keying order in the image grid.
		/// </summary>
		WDEPSOrientation Orientation {get; set;}
	}    

    /// <summary>
	/// An error override collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("3DE1D213-5D34-4BBB-A289-91953CD04ED5")]
    public interface IWDEErrorOverrides : IEnumerable
	{
		/// <summary>
		/// Gets the number of error override in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the error override at the given index.
		/// </summary>
        IWDEErrorOverride this[int index] { get; }
		/// <summary>
		/// Adds an error override to the collection.
		/// </summary>
        /// <param name="ErrorOverride">The erroroverride to add.</param>
        void Add(IWDEErrorOverride ErrorOverride);
		/// <summary>
		/// Removes all error overrides from the collection.
		/// </summary>
		void Clear();
	}

    /// <summary>
    /// An error override.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("FA1C9041-F1C2-4F8E-BD31-5DFE712D9F26")]
    public interface IWDEErrorOverride
    {
        /// <summary>
        /// Gets or sets the error name.
        /// </summary>        
        WDEErrorOverrideType ErrorName { get; set; }
        /// <summary>
        /// Gets or sets a error message.
        /// </summary>
        string ErrorMessage { get; set; }        
    }

	/// <summary>
	/// An edit def collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("2317459C-2499-4f7b-9144-9F38A228837B")]
	public interface IWDEEditDefs : IEnumerable
	{
		/// <summary>
		/// Gets the number of edit defs in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the edit def at the given index.
		/// </summary>
		IWDEEditDef this[int index] {get;}
		/// <summary>
		/// Adds an edit def to the collection.
		/// </summary>
		/// <param name="EditDef">The edit def to add.</param>
		void Add(IWDEEditDef EditDef);
		/// <summary>
		/// Removes all edit defs from the collection.
		/// </summary>
		void Clear();
	}

    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("E39E9611-DFCB-4f9b-87A1-ACE1FBA6222E")]
    public interface IWDEEditDefs_R1 : IWDEEditDefs
    {
        /// <summary>
        /// Removes the given edit def from the collection.
        /// </summary>
        /// <param name="EditDef">The edit def to remove.</param>
        void Delete(IWDEEditDef EditDef);
    }

	/// <summary>
	/// A base edit def.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("98498F0E-DF4F-46f7-AA2D-DDF1156D3DD4")]
	public interface IWDEEditDef
	{
		/// <summary>
		/// Gets the display name.
		/// </summary>
		string DisplayName {get;}
		/// <summary>
		/// Gets the full name.
		/// </summary>
		string FullName {get;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the enabled flag.
		/// </summary>
		bool Enabled {get; set;}
		/// <summary>
		/// Gets or sets an error message.
		/// </summary>
		string ErrorMessage {get; set;}
		/// <summary>
		/// Gets or sets the error type.
		/// </summary>
		WDEEditErrorType ErrorType {get; set;}
		/// <summary>
		/// Gets or sets the session mode this edit will run in.
		/// </summary>
		WDESessionType SessionMode {get; set;}
	}

    /// <summary>
    /// A base edit def
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("342FC671-E580-4322-AFF6-76B14208E77D")]
    public interface IWDEEditDef_R1 : IWDEEditDef
    {
        /// <summary>
        /// Gets or sets the display name of the script object that implements this edit.
        /// </summary>
        string DisplayName { get; set; }
        /// <summary>
        /// Gets or sets the full name of the script object that implements this edit.
        /// </summary>
        string FullName { get; set;}
        /// <summary>
        /// Gets or sets the edit parameters.
        /// </summary>
        string EditParams { get; set;}
    }

    /// <summary>
    /// The internal edit def interface.
    /// </summary>
    internal interface IWDEEditDefInternal
    {
        /// <summary>
        /// Gets or sets the object that implements this edit.
        /// </summary>
        Scripts.IScriptEdit EditObject { get; set;}
    }

	/// <summary>
	/// Check digit edit.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("65FDF050-6961-4566-B26F-1F7ADD4072BF")]
	public interface IWDECheckDigitEditDef : IWDEEditDef
	{
		/// <summary>
		/// Gets the display name.
		/// </summary>
		string DisplayName {get;}
		/// <summary>
		/// Gets the full name.
		/// </summary>
		string FullName {get;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the enabled flag.
		/// </summary>
		bool Enabled {get; set;}
		/// <summary>
		/// Gets or sets an error message.
		/// </summary>
		string ErrorMessage {get; set;}
		/// <summary>
		/// Gets or sets the error type.
		/// </summary>
		WDEEditErrorType ErrorType {get; set;}
		/// <summary>
		/// Gets or sets the session mode this edit will run in.
		/// </summary>
		WDESessionType SessionMode {get; set;}
		/// <summary>
		/// Gets or sets the check digit methods to be used.
		/// </summary>
		WDECheckDigitMethods Methods {get; set;}
	}

	/// <summary>
	/// Range edit.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("EB088E05-BDA7-463b-943E-4E2AF038EFEB")]
	public interface IWDERangeEditDef : IWDEEditDef
	{
		/// <summary>
		/// Gets the display name.
		/// </summary>
		string DisplayName {get;}
		/// <summary>
		/// Gets the full name.
		/// </summary>
		string FullName {get;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the enabled flag.
		/// </summary>
		bool Enabled {get; set;}
		/// <summary>
		/// Gets or sets an error message.
		/// </summary>
		string ErrorMessage {get; set;}
		/// <summary>
		/// Gets or sets the error type.
		/// </summary>
		WDEEditErrorType ErrorType {get; set;}
		/// <summary>
		/// Gets or sets the session mode this edit will run in.
		/// </summary>
		WDESessionType SessionMode {get; set;}
		/// <summary>
		/// Gets or sets the high end of the range.
		/// </summary>
		string HighRange {get; set;}
		/// <summary>
		/// Gets or sets the low end of the range.
		/// </summary>
		string LowRange {get; set;}
	}

	/// <summary>
	/// Required edit.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("DD4849A8-81D4-4965-8472-FAF16B5394F0")]
	public interface IWDERequiredEditDef : IWDEEditDef
	{
		/// <summary>
		/// Gets the display name.
		/// </summary>
		string DisplayName {get;}
		/// <summary>
		/// Gets the full name.
		/// </summary>
		string FullName {get;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the enabled flag.
		/// </summary>
		bool Enabled {get; set;}
		/// <summary>
		/// Gets or sets an error message.
		/// </summary>
		string ErrorMessage {get; set;}
		/// <summary>
		/// Gets or sets the error type.
		/// </summary>
		WDEEditErrorType ErrorType {get; set;}
		/// <summary>
		/// Gets or sets the session mode this edit will run in.
		/// </summary>
		WDESessionType SessionMode {get; set;}
		/// <summary>
		/// Gets or sets a conditional expression.
		/// </summary>
		string Expression {get; set;}
	}

	/// <summary>
	/// Simple list edit.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("B3EFA8BD-01AC-4ab3-A33A-E5009AD89995")]
	public interface IWDESimpleListEditDef : IWDEEditDef
	{
		/// <summary>
		/// Gets the display name.
		/// </summary>
		string DisplayName {get;}
		/// <summary>
		/// Gets the full name.
		/// </summary>
		string FullName {get;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the enabled flag.
		/// </summary>
		bool Enabled {get; set;}
		/// <summary>
		/// Gets or sets an error message.
		/// </summary>
		string ErrorMessage {get; set;}
		/// <summary>
		/// Gets or sets the error type.
		/// </summary>
		WDEEditErrorType ErrorType {get; set;}
		/// <summary>
		/// Gets or sets the session mode this edit will run in.
		/// </summary>
		WDESessionType SessionMode {get; set;}
		/// <summary>
		/// Gets or sets the list of allowed values.
		/// </summary>
		string[] List {get; set;}
	}

	/// <summary>
	/// Valid lengths edit.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("5B813914-E83F-4846-9235-931CF861DE4D")]
	public interface IWDEValidLengthsEditDef : IWDEEditDef
	{
		/// <summary>
		/// Gets the display name.
		/// </summary>
		string DisplayName {get;}
		/// <summary>
		/// Gets the full name.
		/// </summary>
		string FullName {get;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the enabled flag.
		/// </summary>
		bool Enabled {get; set;}
		/// <summary>
		/// Gets or sets an error message.
		/// </summary>
		string ErrorMessage {get; set;}
		/// <summary>
		/// Gets or sets the error type.
		/// </summary>
		WDEEditErrorType ErrorType {get; set;}
		/// <summary>
		/// Gets or sets the session mode this edit will run in.
		/// </summary>
		WDESessionType SessionMode {get; set;}
		/// <summary>
		/// Gets or sets the list of valid lengths.
		/// </summary>
		int[] Lengths {get; set;}
	}

	/// <summary>
	/// Address correction edit def.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("205D8087-90B1-464c-ADF7-C741E26FB810")]
	public interface IWDEAddressCorrectionEditDef : IWDEEditDef
	{
		/// <summary>
		/// Gets the display name.
		/// </summary>
		string DisplayName {get;}
		/// <summary>
		/// Gets the full name.
		/// </summary>
		string FullName {get;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the enabled flag.
		/// </summary>
		bool Enabled {get; set;}
		/// <summary>
		/// Gets or sets an error message.
		/// </summary>
		string ErrorMessage {get; set;}
		/// <summary>
		/// Gets or sets the error type.
		/// </summary>
		WDEEditErrorType ErrorType {get; set;}
		/// <summary>
		/// Gets or sets the session mode this edit will run in.
		/// </summary>
		WDESessionType SessionMode {get; set;}
		/// <summary>
		/// Gets the lookup fields.
		/// </summary>
		IWDEZP4LookupFields LookupFields {get;}
		/// <summary>
		/// Gets the result fields.
		/// </summary>
		IWDEZP4ResultFields ResultFields {get;}
		/// <summary>
		/// Gets or sets a value indicating whether plugged results should be reviewed.
		/// </summary>
		bool ReviewResults {get; set;}
		/// <summary>
		/// Gets or sets a value indicated whether DPV should be used to verify the address.
		/// </summary>
		bool UseDPV {get; set;}
	}

	/// <summary>
	/// Conditional goto edit def.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("22CE43E1-44C0-4b50-8BEF-56F379228EB8")]
	public interface IWDEConditionalGotoEditDef : IWDEEditDef
	{
		/// <summary>
		/// Gets the display name.
		/// </summary>
		string DisplayName {get;}
		/// <summary>
		/// Gets the full name.
		/// </summary>
		string FullName {get;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the enabled flag.
		/// </summary>
		bool Enabled {get; set;}
		/// <summary>
		/// Gets or sets an error message.
		/// </summary>
		string ErrorMessage {get; set;}
		/// <summary>
		/// Gets or sets the error type.
		/// </summary>
		WDEEditErrorType ErrorType {get; set;}
		/// <summary>
		/// Gets or sets the session mode this edit will run in.
		/// </summary>
		WDESessionType SessionMode {get; set;}
		/// <summary>
		/// Gets the goto defs.
		/// </summary>
		IWDEConditionalGotos Gotos {get;}
	}

	/// <summary>
	/// Date edit def.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("2136ED8A-30FB-4a41-88D2-EE7628F2F995")]
	public interface IWDEDateEditDef : IWDEEditDef
	{
		/// <summary>
		/// Gets the display name.
		/// </summary>
		string DisplayName {get;}
		/// <summary>
		/// Gets the full name.
		/// </summary>
		string FullName {get;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the enabled flag.
		/// </summary>
		bool Enabled {get; set;}
		/// <summary>
		/// Gets or sets an error message.
		/// </summary>
		string ErrorMessage {get; set;}
		/// <summary>
		/// Gets or sets the error type.
		/// </summary>
		WDEEditErrorType ErrorType {get; set;}
		/// <summary>
		/// Gets or sets the session mode this edit will run in.
		/// </summary>
		WDESessionType SessionMode {get; set;}
		/// <summary>
		/// Gets or sets a value indicated whether future dates should be considered valid.
		/// </summary>
		bool AllowFutureDates {get; set;}
	}

	/// <summary>
	/// Diagnosis pointer edit def.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("71B93C40-18E7-4117-82B8-895DCF4010F3")]
	public interface IWDEDiagnosisPtrEditDef : IWDEEditDef
	{
		/// <summary>
		/// Gets the display name.
		/// </summary>
		string DisplayName {get;}
		/// <summary>
		/// Gets the full name.
		/// </summary>
		string FullName {get;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the enabled flag.
		/// </summary>
		bool Enabled {get; set;}
		/// <summary>
		/// Gets or sets an error message.
		/// </summary>
		string ErrorMessage {get; set;}
		/// <summary>
		/// Gets or sets the error type.
		/// </summary>
		WDEEditErrorType ErrorType {get; set;}
		/// <summary>
		/// Gets or sets the session mode this edit will run in.
		/// </summary>
		WDESessionType SessionMode {get; set;}
		/// <summary>
		/// Gets the list of diagnosis code fields.
		/// </summary>
		IWDEFieldDefLinks DiagnosisCodes {get;}
		/// <summary>
		/// Gets or sets the database to use for lookup validation.
		/// </summary>
		string Database {get; set;}
		/// <summary>
		/// Gets or sets the query to use for lookup validation.
		/// </summary>
		string Query {get; set;}
	}

	/// <summary>
	/// Balance check edit def.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("B32D101D-5C8C-4243-B280-0CC945E93698")]
	public interface IWDEBalanceCheckEditDef : IWDEEditDef
	{
		/// <summary>
		/// Gets the display name.
		/// </summary>
		string DisplayName {get;}
		/// <summary>
		/// Gets the full name.
		/// </summary>
		string FullName {get;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the enabled flag.
		/// </summary>
		bool Enabled {get; set;}
		/// <summary>
		/// Gets or sets an error message.
		/// </summary>
		string ErrorMessage {get; set;}
		/// <summary>
		/// Gets or sets the error type.
		/// </summary>
		WDEEditErrorType ErrorType {get; set;}
		/// <summary>
		/// Gets or sets the session mode this edit will run in.
		/// </summary>
		WDESessionType SessionMode {get; set;}
		/// <summary>
		/// Gets the list of fields to sum.
		/// </summary>
		IWDEFieldDefLinks SumFields {get;}
		/// <summary>
		/// Gets or sets the acceptable margin of error.
		/// </summary>
		double ErrorMargin {get; set;}
	}

	/// <summary>
	/// A script event definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("A9C765E2-C7F2-4e5d-9D2D-3C7790111FFB")]
	public interface IWDEEventScriptDef
	{
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets a value indicating whether this event is enabled.
		/// </summary>
		bool Enabled {get; set;}
		/// <summary>
		/// Gets or sets the full name of the script object that runs this event.
		/// </summary>
		string ScriptFullName {get; set;}
        IWDEEventScriptDef Clone();
	}

	/// <summary>
	/// A session def collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("4D25CFAF-09D2-463e-BF60-E18B75E70161")]
	public interface IWDESessionDefs : IEnumerable
	{
		/// <summary>
		/// Gets the number of session defs in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the session def at the given index.
		/// </summary>
		IWDESessionDef this[int index] {get;}
		/// <summary>
		/// Searches for a session def by the given name.
		/// </summary>
		/// <param name="SessionDefName">The session def name to search for.</param>
		/// <returns>The index of the matching session def. Returns -1 if there is no match.</returns>
		int Find(string SessionDefName);
		/// <summary>
		/// Creates a session def using the given name.
		/// </summary>
		/// <param name="SessionDefName">The name of the new session def.</param>
		/// <returns>The newly created session def.</returns>
		IWDESessionDef Add(string SessionDefName);
		/// <summary>
		/// Creates a session def using default naming.
		/// </summary>
		/// <returns>The newly created session def.</returns>
		IWDESessionDef Add();
		/// <summary>
		/// Removes all session defs from the collection.
		/// </summary>
		void Clear();
	}

	/// <summary>
	/// A session definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("A18B2C09-3886-4bad-87F7-71F29FB0C350")]
	public interface IWDESessionDef
	{
		/// <summary>
		/// Gets or sets the session def name.
		/// </summary>
		string SessionDefName {get; set;}
		/// <summary>
		/// Gets or sets the default data panel height.
		/// </summary>
		int DataPanelHeight {get; set;}
		/// <summary>
		/// Gets or sets the first image index.
		/// </summary>
		int FirstImage {get; set;}
		/// <summary>
		/// Gets the linked form def collection.
		/// </summary>
		IWDEFormLinks Forms {get;}
		/// <summary>
		/// Gets or sets session options.
		/// </summary>
		WDESessionOption Options {get; set;}
		/// <summary>
		/// Gets the photostitch settings.
		/// </summary>
		IWDEPhotoStitchDef PhotoStitch {get;}
		/// <summary>
		/// Gets or sets a value indicating whether the ticker window should be shown in character repair mode.
		/// </summary>
		bool ShowTicker {get; set;}
		/// <summary>
		/// Gets or sets the ticker window height for character repair mode.
		/// </summary>
		int TickerCharHeight {get; set;}
		/// <summary>
		/// Gets or sets the default image scale.
		/// </summary>
		WDEImageScale ImageScale {get; set;}
		/// <summary>
		/// Gets or sets the image scale percent when ImageScale is set to ScalePercent.
		/// </summary>
		int ImageScalePercent {get; set;}
		/// <summary>
		/// Gets or sets the session type.
		/// </summary>
		WDESessionType SessionType {get; set;}
		/// <summary>
		/// Gets or sets the session style.
		/// </summary>
		WDESessionStyle SessionStyle {get; set;}

		/// <summary>
		/// Gets the form to use for a given image type.
		/// </summary>
		/// <param name="ImageType">The image type to select a form for.</param>
		/// <returns>The form def to use.</returns>
		IWDEFormDef GetFormByImageType(string ImageType);
	}

    /// <summary>
    /// A session definition
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("14A3FF9D-ACCC-4153-98E0-9833B97B695E")]
    public interface IWDESessionDef_R1 : IWDESessionDef
    {
        void SetImageFormMap(string ImageType, IWDEFormDef FormDef);
    }

    /// <summary>
    /// Docking options for session plugins
    /// </summary>
    public enum PluginDocking
    {
        /// <summary>
        /// Dock to the left of the form.
        /// </summary>
        [Description("Dock to the left of the form.")]
        Left,
        /// <summary>
        /// Dock to the right of the form.
        /// </summary>
        [Description("Dock to the right of the form.")]
        Right,
        /// <summary>
        /// Dock on the top side of the form.
        /// </summary>
        [Description("Dock to the top of the form.")]
        Top,
        /// <summary>
        /// Dock on the bottom of the form.
        /// </summary>
        [Description("Dock to the bottom of the form.")]
        Bottom
    }

    /// <summary>
    /// A session definition
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("450B514E-2272-4381-81B4-F45482D5B54D")]
    public interface IWDESessionDef_R2 : IWDESessionDef_R1
    {
        /// <summary>
        /// The assembly file name for the plugin.
        /// </summary>
        string PluginName { get; set; }
        /// <summary>
        /// The docking option for the plugin.
        /// </summary>
        PluginDocking UserPanelDocking { get; set;}
        /// <summary>
        /// The width or height of the plugin depending on the docking option in UserPanelDocking.
        /// </summary>
        int UserPanelWidthHeight { get; set;}
    }

    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("FF49EB89-29DC-4A06-880A-189B24B9C0C9")]
    public interface IWDESessionDef_R3 : IWDESessionDef_R2
    {
        /// <summary>
        /// Gets or sets the toolbar dock options
        /// </summary>
        ToolbarDock ToolbarDock { get; set; }
        /// <summary>
        /// Gets or sets the image dock options
        /// </summary>
        ImageDock ImageDock { get; set; }
    }

	/// <summary>
	/// A form link collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("E24966ED-3D6D-467b-88BF-CAD442124348")]
	public interface IWDEFormLinks : IEnumerable
	{
		/// <summary>
		/// Gets the number of forms in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the form def at the given index.
		/// </summary>
		IWDEFormDef this[int index] {get;}
		/// <summary>
		/// Adds a form def to the collection.
		/// </summary>
		/// <param name="FormDef">The form def to add.</param>
		void Add(IWDEFormDef FormDef);
		/// <summary>
		/// Removes all form defs from the collection.
		/// </summary>
		void Clear();
	}

    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("B2683B3F-7167-4762-AC68-0AA166067BA8")]
    public interface IWDEFormLinks_R1 : IWDEFormLinks
    {
        void RemoveForm(IWDEFormDef FormDef);
    }

	/// <summary>
	/// Lookup fields for the address correction edit.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("ADFC39F6-C70D-47fa-92B0-87544F491CC8")]
	public interface IWDEZP4LookupFields
	{
		/// <summary>
		/// Gets or sets the address field.
		/// </summary>
		IWDEFieldDef AddressField {get; set;}
		/// <summary>
		/// Gets or sets the city field.
		/// </summary>
		IWDEFieldDef CityField {get; set;}
		/// <summary>
		/// Gets or sets the zip code field.
		/// </summary>
		IWDEFieldDef ZipCodeField {get; set;}
	}

	/// <summary>
	/// The result fields for the address correction edit.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("4BB6541D-6F3C-47e5-BF90-09ACB52E132D")]
	public interface IWDEZP4ResultFields
	{
		/// <summary>
		/// Gets or sets the address field.
		/// </summary>
		IWDEFieldDef AddressField {get; set;}
		/// <summary>
		/// Gets or sets the city field.
		/// </summary>
		IWDEFieldDef CityField {get; set;}
		/// <summary>
		/// Gets or sets the state field.
		/// </summary>
		IWDEFieldDef StateField {get; set;}
		/// <summary>
		/// Gets or sets the zip code field.
		/// </summary>
		IWDEFieldDef ZipCodeField {get; set;}
		/// <summary>
		/// Gets or sets the zip plus 4 field.
		/// </summary>
		IWDEFieldDef ZipPlus4Field {get; set;}
		/// <summary>
		/// Gets or sets the house number field.
		/// </summary>
		IWDEFieldDef HouseNumberField {get; set;}
		/// <summary>
		/// Gets or sets the pre-direction field.
		/// </summary>
		IWDEFieldDef PreDirectionField {get; set;}
		/// <summary>
		/// Gets or sets the street name field.
		/// </summary>
		IWDEFieldDef StreetNameField {get; set;}
		/// <summary>
		/// Gets or sets the street suffix field.
		/// </summary>
		IWDEFieldDef StreetSuffixField {get; set;}
		/// <summary>
		/// Gets or sets the post direction field.
		/// </summary>
		IWDEFieldDef PostDirectionField {get; set;}
		/// <summary>
		/// Gets or sets the aparment unit abbreviation field.
		/// </summary>
		IWDEFieldDef AptUnitAbbrField {get; set;}
		/// <summary>
		/// Gets or sets the apartment number field.
		/// </summary>
		IWDEFieldDef AptNumberField {get; set;}
	}

	/// <summary>
	/// A conditional goto collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("F0CC4186-B379-4c16-A44C-4F279487840C")]
	public interface IWDEConditionalGotos : IEnumerable
	{
		/// <summary>
		/// Gets the number of conditional gotos in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the conditional goto at the given index.
		/// </summary>
		IWDEConditionalGoto this[int index] {get;}
		/// <summary>
		/// Creates a conditional goto.
		/// </summary>
		/// <returns>The newly created conditional goto.</returns>
		IWDEConditionalGoto Add();
		/// <summary>
		/// Removes a conditional goto at the given index.
		/// </summary>
		/// <param name="Index">The index to remove.</param>
		void RemoveAt(int Index);
		/// <summary>
		/// Removes all conditional gotos in the collection.
		/// </summary>
		void Clear();
	}

	/// <summary>
	/// A conditional goto definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("7D236EA4-4936-4c46-942E-D7ABF4EBBF56")]
	public interface IWDEConditionalGoto
	{
		/// <summary>
		/// Gets or sets the control to focus.
		/// </summary>
		IWDEControlDef Control {get; set;}
		/// <summary>
		/// Gets or sets the conditional expression.
		/// </summary>
		string Expression {get; set;}
		/// <summary>
		/// Gets or sets a value indicating whether Release should be called.
		/// </summary>
		bool Release {get; set;}
		/// <summary>
		/// Gets or sets a value indicating whether fields between the current control and the destination control should be cleared.
		/// </summary>
		bool ClearFields {get; set;}
	}

	/// <summary>
	/// A table lookup edit def.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("9F634E07-50DC-4e7d-B395-9790EB0DEFEB")]
	public interface IWDETableLookupEditDef : IWDEEditDef
	{
		/// <summary>
		/// Gets the display name.
		/// </summary>
		string DisplayName {get;}
		/// <summary>
		/// Gets the full name.
		/// </summary>
		string FullName {get;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the enabled flag.
		/// </summary>
		bool Enabled {get; set;}
		/// <summary>
		/// Gets or sets an error message.
		/// </summary>
		string ErrorMessage {get; set;}
		/// <summary>
		/// Gets or sets the error type.
		/// </summary>
		WDEEditErrorType ErrorType {get; set;}
		/// <summary>
		/// Gets or sets the session mode this edit will run in.
		/// </summary>
		WDESessionType SessionMode {get; set;}
		/// <summary>
		/// Gets or sets the execute option for this edit def.
		/// </summary>
		WDEExecuteOption ExecuteOn {get; set;}
		/// <summary>
		/// Gets the lookups collection.
		/// </summary>
		IWDETableLookups TableLookups {get;}
	}

	/// <summary>
	/// A table lookup collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("8D182FD0-C0C8-432f-B3B7-C5EABF4415D1")]
	public interface IWDETableLookups : IEnumerable
	{
		/// <summary>
		/// Gets the number of lookups in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the lookup at the given index.
		/// </summary>
		IWDETableLookup this[int index] {get;}
		/// <summary>
		/// Creates a new lookup.
		/// </summary>
		/// <returns>The newly created lookup.</returns>
		IWDETableLookup Add();
		/// <summary>
		/// Removes all lookups from the collection.
		/// </summary>
		void Clear();
	}

	/// <summary>
	/// A table lookup definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("B9B74DA2-DF9E-4b04-8483-E38726872088")]
	public interface IWDETableLookup
	{
		/// <summary>
		/// Gets or sets the database to use.
		/// </summary>
		string Database {get; set;}
		/// <summary>
		/// Gets or sets the database query.
		/// </summary>
		string Query {get; set;}
		/// <summary>
		/// Gets the result fields collection.
		/// </summary>
		IWDELookupResultFields ResultFields {get;}
		/// <summary>
		/// Gets or sets table lookup options.
		/// </summary>
		WDETableLookupOption Options {get; set;}
	}

	/// <summary>
	/// A table lookup result fields collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("B30065E0-A918-4230-B131-3F7148C7A0BF")]
	public interface IWDELookupResultFields : IEnumerable
	{
		/// <summary>
		/// Gets the number of result fields in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the result field at the given index.
		/// </summary>
		IWDELookupResultField this[int index] {get;}
		/// <summary>
		/// Creates a result field.
		/// </summary>
		/// <returns>The newly created result field.</returns>
		IWDELookupResultField Add();
		/// <summary>
		/// Removes all result fields from the collection.
		/// </summary>
		void Clear();
	}

	/// <summary>
	/// A table lookup result field definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("BED3A1D1-4C59-403c-A2B9-16E77B76F82D")]
	public interface IWDELookupResultField
	{
		/// <summary>
		/// Gets or sets the name of the database field.
		/// </summary>
		string DBFieldName {get; set;}
		/// <summary>
		/// Gets or sets the field def.
		/// </summary>
		IWDEFieldDef Field {get; set;}
		/// <summary>
		/// Gets or sets result field options.
		/// </summary>
		WDELookupResultFieldOption Options {get; set;}
	}

	/// <summary>
	/// A zone def collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("91EF0120-FC5D-41af-8BFE-B4C0539B517B")]
	public interface IWDEZoneDefs : IEnumerable
	{
		/// <summary>
		/// Gets the number of zone defs in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the zone def at the given index.
		/// </summary>
		IWDEZoneDef this[int index] {get;}
		/// <summary>
		/// Creates a zone using the given name.
		/// </summary>
		/// <param name="ZoneName">The name of the zone to create.</param>
		/// <returns>The newly created zone.</returns>
		IWDEZoneDef Add(string ZoneName);
		/// <summary>
		/// Creates a zone using default naming.
		/// </summary>
		/// <returns>The newly created zone.</returns>
		IWDEZoneDef Add();
		/// <summary>
		/// Searches for a zone by the given name.
		/// </summary>
		/// <param name="ZoneName">The name to search for.</param>
		/// <returns>The index of the matching zone. Returns -1 if there is no match.</returns>
		int Find(string ZoneName);
		/// <summary>
		/// Removes all zones from the collection.
		/// </summary>
		void Clear();
        void Add(IWDEZoneDef newItem);
	}

	/// <summary>
	/// The base zone definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("A2BBCDC8-6DD1-403a-A9A7-2F04280BB420")]
	public interface IWDEBaseZoneDef
	{
		/// <summary>
		/// Gets or sets the zone name.
		/// </summary>
		string Name {get; set;}
	}

	/// <summary>
	/// A typical zone def.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("54B449EB-8A9B-4f71-9596-475D5CFD5876")]
	public interface IWDEZoneDef : IWDEBaseZoneDef
	{
		/// <summary>
		/// Gets or sets the zone name.
		/// </summary>
		string Name {get; set;}
		/// <summary>
		/// Gets or sets the zone rectangle in image coordinates.
		/// </summary>
		Rectangle ZoneRect {get; set;}
        IWDEZoneDef Clone();
	}

	/// <summary>
	/// A collection of snippet defs.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("85466A78-AA24-408c-8303-D2C81EFAE1C7")]
	public interface IWDESnippetDefs : IEnumerable
	{
		/// <summary>
		/// Gets the number of snippet defs in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the snippet def at the given index.
		/// </summary>
		IWDESnippetDef this[int index] {get;}
		/// <summary>
		/// Creates a snippet def using the given name.
		/// </summary>
		/// <param name="SnippetName">The name of the snippet to create.</param>
		/// <returns>The newly created snippet def.</returns>
		IWDESnippetDef Add(string SnippetName);
		/// <summary>
		/// Creates a snippet def using default naming.
		/// </summary>
		/// <returns>The newly created snippet def.</returns>
		IWDESnippetDef Add();
        void Add(IWDESnippetDef newDef);
        /// <summary>
		/// Searches for a snippet def using the given name.
		/// </summary>
		/// <param name="SnippetName">The name to search for.</param>
		/// <returns>The index of the matching snippet def. Returns -1 if there is no match.</returns>
		int Find(string SnippetName);
		/// <summary>
		/// Removes all snippet defs from the collection.
		/// </summary>
		void Clear();
	}

	/// <summary>
	/// A snippet definition.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("E58B0902-CFE9-46b8-984A-E37A5535E4E0")]
	public interface IWDESnippetDef : IWDEBaseZoneDef
	{
		/// <summary>
		/// Gets or sets the zone name.
		/// </summary>
		string Name {get; set;}
		/// <summary>
		/// The snippet rectangle in image coordinates.
		/// </summary>
		Rectangle SnippetRect {get; set;}
		/// <summary>
		/// The location of the snippet in the snippet view.
		/// </summary>
		Point Location {get; set;}
        IWDESnippetDef Clone();
	}

	/// <summary>
	/// A collection of detail zone defs.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("E548D50A-495E-4d29-9EE2-2191F7FCEEEF")]
	public interface IWDEDetailZoneDefs : IEnumerable
	{
		/// <summary>
		/// Gets the number of detail zone defs in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the detail zone def at the given index.
		/// </summary>
		IWDEDetailZoneDef this[int index] {get;}
		/// <summary>
		/// Creates a detail zone using the given name.
		/// </summary>
		/// <param name="DetailZoneName">The name of the detail zone to create.</param>
		/// <returns>The newly created detail zone.</returns>
		IWDEDetailZoneDef Add(string DetailZoneName);
		/// <summary>
		/// Creates a detail zone using default naming.
		/// </summary>
		/// <returns>The newly created detail zone.</returns>
		IWDEDetailZoneDef Add();
		/// <summary>
		/// Searches for a detail zone by the given name.
		/// </summary>
		/// <param name="DetailZoneName">The name to search for.</param>
		/// <returns>The index of the matching zone. Returns -1 if there is no match.</returns>
		int Find(string DetailZoneName);
		/// <summary>
		/// Removes all detail zones from the collection.
		/// </summary>
		void Clear();
        void Add(IWDEDetailZoneDef newDef);
	}

	/// <summary>
	/// A detail zone def.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("9A6B633C-DC5A-4ed0-9476-907E986E5D3E")]
	public interface IWDEDetailZoneDef : IWDEBaseZoneDef
	{
		/// <summary>
		/// Gets or sets the zone name.
		/// </summary>
		string Name {get; set;}
		/// <summary>
		/// Gets or sets the number of lines in the detail zone def.
		/// </summary>
		int LineCount {get; set;}
		/// <summary>
		/// Gets the height of each zone in the zone def.
		/// </summary>
		int LineHeight {get; set;}
		/// <summary>
		/// Gets the child zone def collection.
		/// </summary>
		IWDEZoneDefs ZoneDefs {get;}
        IWDEDetailZoneDef Clone();
	}

    /// <summary>
    /// A detail zone def.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("3F638351-6F70-4019-914D-B11527F7D504")]
    public interface IWDEDetailZoneDef_R1 : IWDEDetailZoneDef
    {
        int Width { get; set;}
    }

	internal interface IWDEDetailZoneDefInternal
	{
		Rectangle ZoneRect {get; set;}
	}

	/// <summary>
	/// A zip lookup edit def.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("5C7F1F43-1136-43d2-87E9-29ABCFBE30B2")]
	public interface IWDEZipLookupEditDef : IWDEEditDef
	{
		/// <summary>
		/// Gets the display name.
		/// </summary>
		string DisplayName {get;}
		/// <summary>
		/// Gets the full name.
		/// </summary>
		string FullName {get;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the enabled flag.
		/// </summary>
		bool Enabled {get; set;}
		/// <summary>
		/// Gets or sets an error message.
		/// </summary>
		string ErrorMessage {get; set;}
		/// <summary>
		/// Gets or sets the error type.
		/// </summary>
		WDEEditErrorType ErrorType {get; set;}
		/// <summary>
		/// Gets or sets the session mode this edit will run in.
		/// </summary>
		WDESessionType SessionMode {get; set;}
		/// <summary>
		/// Gets or sets the city field.
		/// </summary>
		IWDEFieldDef CityField {get; set;}
		/// <summary>
		/// Gets or sets the city code field.
		/// </summary>
		IWDEFieldDef CityCodeField {get; set;}
		/// <summary>
		/// Gets or sets zip lookup options.
		/// </summary>
		WDEZipLookupOption Options {get; set;}
		/// <summary>
		/// Gets or sets the state field.
		/// </summary>
		IWDEFieldDef StateField {get; set;}
		/// <summary>
		/// Gets or sets the zip code field.
		/// </summary>
		IWDEFieldDef ZipCodeField {get; set;}
	}

	/// <summary>
	/// A validate edit def.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("B420DF49-44E2-4c9e-A870-02F7DAD3AC3D")]
	public interface IWDEValidateEditDef : IWDEEditDef
	{
		/// <summary>
		/// Gets the display name.
		/// </summary>
		string DisplayName {get;}
		/// <summary>
		/// Gets the full name.
		/// </summary>
		string FullName {get;}
		/// <summary>
		/// Gets or sets a description.
		/// </summary>
		string Description {get; set;}
		/// <summary>
		/// Gets or sets the enabled flag.
		/// </summary>
		bool Enabled {get; set;}
		/// <summary>
		/// Gets or sets an error message.
		/// </summary>
		string ErrorMessage {get; set;}
		/// <summary>
		/// Gets or sets the error type.
		/// </summary>
		WDEEditErrorType ErrorType {get; set;}
		/// <summary>
		/// Gets or sets the session mode this edit will run in.
		/// </summary>
		WDESessionType SessionMode {get; set;}
		/// <summary>
		/// Gets the validations collection.
		/// </summary>
		IWDEValidations Validations {get;}
	}

	/// <summary>
	/// A validations collection.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("A9ED8475-4D7D-43f1-B5A5-EC5FCE9A669C")]
	public interface IWDEValidations
	{
		/// <summary>
		/// Gets the number of validations in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the validation at the given index.
		/// </summary>
		IWDEValidation this[int index] {get;}
		/// <summary>
		/// Creates a validation.
		/// </summary>
		/// <returns>The newly created validation.</returns>
		IWDEValidation Add();
		/// <summary>
		/// Removes all validations from the collection.
		/// </summary>
		void Clear();
	}

	/// <summary>
	/// A validation
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("B71D3EDC-D025-4b9f-9BE1-A9DD9DB9E580")]
	public interface IWDEValidation
	{
		/// <summary>
		/// Gets or sets the expression to run.
		/// </summary>
		string Expression {get; set;}
		/// <summary>ddd
		/// Gets or sets an error message.
		/// </summary>
		string ErrorMessage {get; set;}
	}
}
