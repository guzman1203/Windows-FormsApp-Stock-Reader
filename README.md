Make sure that methods like readCandlesticksFromFile, filterCandlesticks, update, normalize, and displayCandlesticks come in 2 styles. One version should be the version that takes arguments and returns something, and the other version should take no argument and return void. For example, filterCandlesticks should have:

Version 1:

List<Candlestick> filterCandlesticks(List<Candlestick> unfilteredList, DateTime startDate, DateTime endDate)

This version needs to be given the unfiltered list and the starting and ending dates of interest. Then this version should return a list of candlesticks.

Version 2:

void filterCandlesticks().

The 2nd version should simply call the 1st version by passing it the necessary arguments and returning the filtered list of candlesticks as in:

void filterCandlesticks()

{

  // Go filter the Form's listOfCandlesticks and put the result in a temporary variable named filteredList

  List<Candlestick> filteredList = filterCandlesticks(listOfCandlesticks, DateTimePicker_Start.Value, DateTimePicker_End.Value);

  // Then set the boundCandlesticks by instantiating a BindlingList from the filtered list as:

  boundCandlesticks = new BindingList(filteredList);

}

3) Make sure that you use the full dynamic range of your chart by setting the Minimum and Maximum of the Axis Y of the OHLC ChartArea. As explained in class, you can find the maximum high and minimum low of the candlesticks in the filtered list. Once you have found the maximum and minimum values of the candlesticks you can add 2% to the Maximum and subtract 2% to the Minimum to set the maximum and minimum values of the Y axis.

Not using as much of the chart range will cost you 10 points. 

Suggestion: Write a function: void normalize() which will set the Axis Y's minimum and maximum.

Ultimately, your FileOK event handler should look something like this:

FileHandler(.......)

{

   readCandlesticksFromFile()

   filterCandlesticks()

   normalizeChart()

   displayChart()

}
