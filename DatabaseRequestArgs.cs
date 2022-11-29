namespace Fofx
{
    public class DatabaseRequestArgs
    {
        #region Readonly Properties

        public ITimeSeriesIterator Iterator { get; private set; }
        public DataManipulation.Interpolation Interpolation{ get; private set; }
        public DataManipulation.Extrapolation Extrapolation { get; private set; }
        public int SourceID { get; private set; }
        public int RelationshipValueID { get; private set; }
        public ITranslator Translator { get; private set; }

        #endregion

        #region Public Ctor

        public DatabaseRequestArgs(TimeSeriesRequest request, ITranslator translator)
        {
            Iterator = request.Iterator;
            if (request.TimeSeries != null)
            {
                Interpolation = request.TimeSeries.Interpolation;
                Extrapolation = request.TimeSeries.Extrapolation;

                if (request.TimeSeries.Source != null)
                    SourceID = request.TimeSeries.Source.SourceID;
            }

            Translator = translator;
        }

        public DatabaseRequestArgs(RelationshipTimeSeriesRequest request, ITranslator translator)
        {
            Iterator = request.Iterator;
            if (request.TimeSeries != null)
            {
                Interpolation = request.TimeSeries.Interpolation;
                Extrapolation = request.TimeSeries.Extrapolation;

                if (request.TimeSeries.Source != null)
                    SourceID = request.TimeSeries.Source.SourceID;
            }

            if (request.Relationship != null && request.Relationship.ValueDefinition != 0)
                RelationshipValueID = request.Relationship.ValueDefinition;

            Translator = translator;
        }

        #endregion
    }
}
