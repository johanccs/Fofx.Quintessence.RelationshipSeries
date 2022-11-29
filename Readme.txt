
#Johan Potgieter 
#2022-11-29
1. RelationshipArrayStringValueRequestHelper - 
	1.1 Wrap parameters of void Read method. Consider create a class to encapsulate the parameters
	1.2 Indent whole file to 4 spaces
	1.3 Remove unused using statements
	1.4 GetDataReader - empty method---------------------------------------------------------------------*
	1.5 place methods in order of public, protected and private
	1.6 Add regions
	1.7 Seal the class

2. TimeSeriesDatabaseContext
	2.1 Indent whole file to 4 spaces
	2.2 Add regions
	2.3 Add curly brace in AddQuickLookups method. Improve readability.

3. DatabaseRequestArgs
	3.1 Indent whole file to 4 spaces
	3.2 Add regions
	3.3 Fields should not be public. Create public properties.
	3.4 Remove readonly keyword but make the property readonly by adding private before the set keyword. This is possible since the properties are populated in the constructor.
	3.5 Remove unused using statements

4. EntityDescriptor
	4.1 Replace public fields with properties;
	4.2 Add regions
	4.3 Arrange code in the foloowing order: properties, constructor, methods
	4.4 throw new NotImplementedException(); ------------------------------------------------------------*

5. Move interfaces into own folder
6. Include line breaks

7. RelationshipDescriptor
	7.1 Implement the IComparable interface

8. RelationshipTimeSeriesKey
	8.1 Implement the IComparable interface

9. RelationshipTimeSeriesRequest
	9.1 Implement the IComparable interface

10. Revision
	10.1 Replace public fields with public properties
	10.2 throw new NotImplementedException();---------------------------------------------------------------*

11. TimeSeriesRequest
	11.1 Implement the IComparable interface

12. RelationshipArrayDateValueRequestHelper
	12.1 Wrap the Read method arguments to second line for imprved readability. Consider create a class to encapsulate the parameters--------------------------#
	12.2 Indent whole file to 4 spaces.
	12.3 Extract magic strings to a constant variable.
	12.4 Break the Read method into smaller methods to imprve readability.
	12.5 Break the While loop in the Read method in to small methods.
	12.6 Wrap all method parameters.
	12.7 Add regions for readability.

13. RelationshipDateCharacteristicRequestHelper
	13.1 Indent whole file to 4 spaces.
	13.2 Add regions.
	13.3 Wrap the Read method arguments to second line for imprved readability. Consider create a class to encapsulate the parameters .
	13.4 Extract magic strings to a constant variable.

14. RelationshipRevisableDateValueRequestHelper
	14.1 Indent whole file to 4 spaces.
	14.2 Add regions.
	14.3 Wrap the Read method arguments to second line for imprved readability. Consider create a class to encapsulate the parameters .
	14.4 Extract magic strings to a constant variable.
	14.5 Break the Read method into smaller methods to imprve readability.
	14.6 Break the While loop in the Read method in to small methods.

15. RelationshipArrayEnumValueRequestHelper
	15.1 Indent whole file to 4 spaces.
	15.2 Wrap the Read method arguments to second line for imprved readability. Consider create a class to encapsulate the parameters .
	15.3 Extract magic strings to a constant variable.

16. Extract TimeSeriesLoadConstant ("[TimeSeries].[RelationshipRevisableEnumTimeseriesLoad]") into AppConstants file as a constant. All classes using the magic value
	can use the contant. If there is a need to change string value, only 1 place needs to be changed.

17. RelationshipArrayDateValueRequestHelper
	17.1 Indent whole file to 4 spaces.
	17.2 Wrap the Read method arguments to second line for imprved readability. Consider create a class to encapsulate the parameters .
	17.3 Extract magic strings to a constant variable.
	17.4 Break the Read method into smaller methods to imprve readability.
	17.5 Break the While loop in the Read method in to small methods.
	17.6 Add regions

18. RelationshipEnumCharacteristicRequestHelper / RelationshipArrayEnumValueRequestHelper / RelationshipRevisableEnumValueRequestHelper /
	RelationshipArrayNumericValueRequestHelper / RelationshipNumericCharacteristicRequestHelper / RelationshipRevisableNumericValueRequestHelper
	RelationshipStringCharacteristicRequestHelper / RelationshipRevisableStringValueRequestHelper
	18.1 Indent whole file to 4 spaces.
	18.2 Add regions
	18.3 Wrap the Read method arguments to second line for imprved readability. Consider create a class to encapsulate the parameters.
	18.4 Extract magic strings to a constant variable.

19. CharacteristicRevisableGeneric / ConstituentRevisableTimeSeriesGeneric
	19.1 Ensure proper spacing between methods.
	19.2 Wrap method arguments.
	19.3 Arrange class layout -> Properties / Ctor / Methods 
	
