using Fofx.Quintessence.RelationshipSeries.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Fofx
{
    public class RelationshipRevisableDateValueRequestHelper : BaseRelationshipRevisableRequestHelper, ITimeSeriesRequestHelper
    {
        #region Public methods

        public override INullableReader GetDataReader(int[] entites, int[] factors, DatabaseRequestArgs args)
        {
            throw new NotImplementedException("");
        }

        public override INullableReader GetDataReader(int[] entites, int[] factors, int[] relationships, DatabaseRequestArgs args)
        {
            SqlParameter[] parameters = GetParameters(entites, factors, relationships, args);
            return DataAccess.GetDataReader(AppConstants.TimeSeriesLoadConstant, parameters);
        }

        public override ITimeSeries CreateNew(ITimeSeriesIterator iterator, TimeSeriesDescriptor factor)
        {
            return new DateConstituentRevisableTimeSeries();
        }

        public override void Add(ITimeSeries iTimeSeries, INullableReader reader, DatabaseRequestArgs args, TimeSeriesDatabaseContext requester)
        {
            int toEntityID = reader.GetInt32(2);
            DateTime valueDate = reader.GetDateTime(3);
            DateTime declarationDate = reader.GetDateTime(4);
            string value = reader.GetString(5);
            int? nonKeyedAttributeSetId = reader.GetNullableInt32(7);

            NonKeyedAttributeSet nonKeyedAttributeSet = null;
            if (nonKeyedAttributeSetId != null)
                nonKeyedAttributeSet = args.Translator.GetNonKeyedAttributeSet((int)nonKeyedAttributeSetId);


            IEntityDescriptor entity = null;
            if (!requester.EntityLookup.TryGetValue(toEntityID, out entity))
            {
                if (!args.Translator.TryGetEntityDescriptorByID(toEntityID, out entity))
                    entity = new EntityDescriptor(toEntityID);
            }
          ((DateConstituentRevisableTimeSeries)iTimeSeries).Add(entity, valueDate, declarationDate, value, nonKeyedAttributeSet);
        }

        #endregion

        #region Protected methods

        protected override void Read(int[] entities, int[] timeseries, int[] relationship, DatabaseRequestArgs args,
                                     TimeSeriesDatabaseContext requester, Dictionary<QuickID, ITimeSeriesKey> keys,
                                     SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult)
        {
            DateConstituentRevisableTimeSeries ncs = null;
            ITimeSeries referenceTimeSeries = null;
            ITimeSeriesKey referenceKey = null;

            Preference codeTypePreference = null;
            int previousEntityID = -1;
            int previousTimeSeriesValueID = -1;
            int previousRelationshipValueID = -1;
            DateTime previousValueDate = DateTime.MinValue;
            DateTime previousDeclarationDate = DateTime.MinValue;

            SortedList<DateTime, Revision<ConstituentDatePoint>> pointCollection = null;
            Revision<ConstituentDatePoint> revisionCollection = null;
            ConstituentDatePoint constituentPoint = null;

            using (INullableReader reader = GetDataReader(entities, timeseries, relationship, args))
            {
                Read(args, requester, keys, dbTimeSeriesResult, ref ncs, ref referenceTimeSeries, ref referenceKey,
                     ref codeTypePreference, ref previousEntityID, ref previousTimeSeriesValueID,
                     ref previousRelationshipValueID, ref previousValueDate, ref previousDeclarationDate,
                     ref pointCollection, ref revisionCollection, ref constituentPoint, reader);
            }

            QuickID qid2 = new QuickID(previousEntityID, previousTimeSeriesValueID, previousRelationshipValueID);
            if (keys.TryGetValue(qid2, out referenceKey))
            {
                if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                {
                    ncs = referenceTimeSeries as DateConstituentRevisableTimeSeries;
                    if (ncs != null)
                    {
                        ncs.Add(pointCollection);
                    }
                }
            }
        }

        #endregion

        #region Private region

        private void Read(DatabaseRequestArgs args, TimeSeriesDatabaseContext requester,
                                 Dictionary<QuickID, ITimeSeriesKey> keys,
                                 SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult,
                                 ref DateConstituentRevisableTimeSeries ncs, ref ITimeSeries referenceTimeSeries,
                                 ref ITimeSeriesKey referenceKey, ref Preference codeTypePreference,
                                 ref int previousEntityID, ref int previousTimeSeriesValueID,
                                 ref int previousRelationshipValueID, ref DateTime previousValueDate,
                                 ref DateTime previousDeclarationDate,
                                 ref SortedList<DateTime, Revision<ConstituentDatePoint>> pointCollection,
                                 ref Revision<ConstituentDatePoint> revisionCollection,
                                 ref ConstituentDatePoint constituentPoint, INullableReader reader)
        {
            while (reader.Read())
            {
                int entityID = reader.GetInt32(0);
                int timeSeriesValueID = reader.GetInt32(1);
                int relationshipValueID = reader.GetInt32(2);
                int toEntityID = reader.GetInt32(3);
                DateTime valueDate = reader.GetDateTime(4);
                DateTime declarationDate = reader.GetDateTime(5);
                string value = reader.GetNullableString(6);
                int? nonKeyedAttributeSetId = reader.GetNullableInt32(7);

                NonKeyedAttributeSet nonKeyedAttributeSet = null;
                if (nonKeyedAttributeSetId != null)
                    nonKeyedAttributeSet = args.Translator.GetNonKeyedAttributeSet((int)nonKeyedAttributeSetId);

                IEntityDescriptor entity = null;

                Validate(requester, keys, dbTimeSeriesResult, ref ncs, ref referenceTimeSeries, ref referenceKey,
                         ref codeTypePreference, ref previousEntityID, ref previousTimeSeriesValueID,
                         ref previousRelationshipValueID, ref previousValueDate, ref previousDeclarationDate,
                         ref pointCollection, ref revisionCollection, ref constituentPoint, entityID, timeSeriesValueID,
                         relationshipValueID, valueDate, declarationDate, nonKeyedAttributeSet);

                if (!requester.EntityLookup.TryGetValue(toEntityID, out entity))
                {
                    if (!args.Translator.TryGetEntityDescriptorByID(toEntityID, codeTypePreference, out entity))
                        entity = new EntityDescriptor(toEntityID);
                }

                constituentPoint.Add(entity, value, nonKeyedAttributeSet);
            }
        }

        private void Validate(TimeSeriesDatabaseContext requester, Dictionary<QuickID, ITimeSeriesKey> keys,
                                     SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult,
                                     ref DateConstituentRevisableTimeSeries ncs, ref ITimeSeries referenceTimeSeries,
                                     ref ITimeSeriesKey referenceKey, ref Preference codeTypePreference,
                                     ref int previousEntityID, ref int previousTimeSeriesValueID,
                                     ref int previousRelationshipValueID, ref DateTime previousValueDate,
                                     ref DateTime previousDeclarationDate,
                                     ref SortedList<DateTime, Revision<ConstituentDatePoint>> pointCollection,
                                     ref Revision<ConstituentDatePoint> revisionCollection,
                                     ref ConstituentDatePoint constituentPoint, int entityID, int timeSeriesValueID,
                                     int relationshipValueID, DateTime valueDate, DateTime declarationDate,
                                     NonKeyedAttributeSet nonKeyedAttributeSet)
        {
            if (entityID != previousEntityID || previousTimeSeriesValueID != timeSeriesValueID || previousRelationshipValueID != relationshipValueID)
            {
                QuickID qid = new QuickID(entityID, timeSeriesValueID, relationshipValueID);
                if (keys.TryGetValue(qid, out referenceKey))
                {
                    RelationshipTimeSeriesKey tsk = referenceKey as RelationshipTimeSeriesKey;
                    if (tsk == null || tsk.Relationship == null || tsk.Relationship.Source == null)
                        codeTypePreference = null;
                    else
                    {
                        if (codeTypePreference != null && tsk.Relationship.Source.CodePreference != null)
                        {
                            if (codeTypePreference.Id != tsk.Relationship.Source.CodePreference.Id)
                                requester.EntityLookup.Clear();

                            codeTypePreference = tsk.Relationship.Source.CodePreference;
                        }
                        else
                        {
                            requester.EntityLookup.Clear();
                            codeTypePreference = tsk.Relationship.Source.CodePreference;
                        }
                    }
                }

                qid = new QuickID(previousEntityID, previousTimeSeriesValueID, previousRelationshipValueID);
                if (keys.TryGetValue(qid, out referenceKey))
                {
                    if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                    {
                        ncs = referenceTimeSeries as DateConstituentRevisableTimeSeries;
                        if (ncs != null)
                        {
                            ncs.Add(pointCollection);
                        }
                    }
                }

                ValidateIDs(out previousEntityID, out previousTimeSeriesValueID, out previousRelationshipValueID,
                            out previousValueDate, out previousDeclarationDate, out pointCollection,
                            out revisionCollection, out constituentPoint, entityID, timeSeriesValueID,
                            relationshipValueID, valueDate, declarationDate, nonKeyedAttributeSet);
            }
            else if (valueDate != previousValueDate)
            {
                ValidateValueDate(out previousValueDate, out previousDeclarationDate, pointCollection,
                                  out revisionCollection, out constituentPoint, valueDate, declarationDate,
                                  nonKeyedAttributeSet);
            }
            else if (declarationDate != previousDeclarationDate)
            {
                ValidateDeclarationDate(out previousValueDate, out previousDeclarationDate, revisionCollection,
                                        out constituentPoint, valueDate, declarationDate, nonKeyedAttributeSet);
            }
        }

        private static void ValidateIDs(out int previousEntityID, out int previousTimeSeriesValueID,
                                        out int previousRelationshipValueID, out DateTime previousValueDate,
                                        out DateTime previousDeclarationDate,
                                        out SortedList<DateTime, Revision<ConstituentDatePoint>> pointCollection,
                                        out Revision<ConstituentDatePoint> revisionCollection,
                                        out ConstituentDatePoint constituentPoint, int entityID, int timeSeriesValueID,
                                        int relationshipValueID, DateTime valueDate, DateTime declarationDate,
                                        NonKeyedAttributeSet nonKeyedAttributeSet)
        {
            pointCollection = new SortedList<DateTime, Revision<ConstituentDatePoint>>();
            revisionCollection = new Revision<ConstituentDatePoint>(valueDate);
            pointCollection.Add(valueDate, revisionCollection);
            constituentPoint = new ConstituentDatePoint(declarationDate, valueDate, nonKeyedAttributeSet);
            revisionCollection.Add(declarationDate, constituentPoint);
            previousEntityID = entityID;
            previousTimeSeriesValueID = timeSeriesValueID;
            previousRelationshipValueID = relationshipValueID;
            previousValueDate = valueDate;
            previousDeclarationDate = declarationDate;
        }

        private static void ValidateDeclarationDate(out DateTime previousValueDate, out DateTime previousDeclarationDate,
                                                    Revision<ConstituentDatePoint> revisionCollection,
                                                    out ConstituentDatePoint constituentPoint, DateTime valueDate,
                                                    DateTime declarationDate, NonKeyedAttributeSet nonKeyedAttributeSet)
        {
            constituentPoint = new ConstituentDatePoint(declarationDate, valueDate, nonKeyedAttributeSet);
            revisionCollection.Add(declarationDate, constituentPoint);
            previousValueDate = valueDate;
            previousDeclarationDate = declarationDate;
        }

        private void ValidateValueDate(out DateTime previousValueDate, out DateTime previousDeclarationDate,
                                       SortedList<DateTime, Revision<ConstituentDatePoint>> pointCollection,
                                       out Revision<ConstituentDatePoint> revisionCollection,
                                       out ConstituentDatePoint constituentPoint, DateTime valueDate,
                                       DateTime declarationDate, NonKeyedAttributeSet nonKeyedAttributeSet)
        {
            revisionCollection = new Revision<ConstituentDatePoint>(valueDate);
            pointCollection.Add(valueDate, revisionCollection);
            constituentPoint = new ConstituentDatePoint(declarationDate, valueDate, nonKeyedAttributeSet);
            revisionCollection.Add(declarationDate, constituentPoint);
            previousValueDate = valueDate;
            previousDeclarationDate = declarationDate;
        }

        #endregion
    }
}
