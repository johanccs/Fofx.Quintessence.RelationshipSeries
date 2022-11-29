using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Fofx.Quintessence.RelationshipSeries.Helpers;

namespace Fofx
{
    public class RelationshipArrayEnumValueRequestHelper : BaseRelationshipRevisableRequestHelper, ITimeSeriesRequestHelper
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
            return new TextConstituentArrayTimeSeries();
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
          ((TextConstituentArrayTimeSeries)iTimeSeries).Add(entity, valueDate, declarationDate, value, nonKeyedAttributeSet);
        }

        #endregion

        #region Protected Methods

        protected override void Read(int[] entities, int[] timeseries, int[] relationship, DatabaseRequestArgs args,
                                     TimeSeriesDatabaseContext requester, Dictionary<QuickID, ITimeSeriesKey> keys,
                                     SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult)
        {
            TextConstituentArrayTimeSeries ncs = null;
            ITimeSeries referenceTimeSeries = null;
            ITimeSeriesKey referenceKey = null;

            Preference codeTypePreference = null;
            int previousEntityID = -1;
            int previousTimeSeriesValueID = -1;
            int previousRelationshipValueID = -1;
            DateTime previousValueDate = DateTime.MinValue;
            DateTime previousDeclarationDate = DateTime.MinValue;

            SortedList<DateTime, Revision<ConstituentTextArrayPoint>> pointCollection = null;
            Revision<ConstituentTextArrayPoint> revisionCollection = null;
            ConstituentTextArrayPoint constituentPoint = null;

            using (INullableReader reader = GetDataReader(entities, timeseries, relationship, args))
            {
                DoValidation(args, requester, keys, dbTimeSeriesResult, ref ncs, ref referenceTimeSeries, ref referenceKey, ref codeTypePreference, ref previousEntityID, ref previousTimeSeriesValueID, ref previousRelationshipValueID, ref previousValueDate, ref previousDeclarationDate, ref pointCollection, ref revisionCollection, ref constituentPoint, reader);
            }

            QuickID qid2 = new QuickID(previousEntityID, previousTimeSeriesValueID, previousRelationshipValueID);
            if (keys.TryGetValue(qid2, out referenceKey))
            {
                if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                {
                    ncs = referenceTimeSeries as TextConstituentArrayTimeSeries;
                    if (ncs != null)
                    {
                        ncs.Add(pointCollection);
                    }
                }
            }
        }

        #endregion

        #region Private methods

        private static void DoValidation(DatabaseRequestArgs args, TimeSeriesDatabaseContext requester, Dictionary<QuickID, ITimeSeriesKey> keys, SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult, ref TextConstituentArrayTimeSeries ncs, ref ITimeSeries referenceTimeSeries, ref ITimeSeriesKey referenceKey, ref Preference codeTypePreference, ref int previousEntityID, ref int previousTimeSeriesValueID, ref int previousRelationshipValueID, ref DateTime previousValueDate, ref DateTime previousDeclarationDate, ref SortedList<DateTime, Revision<ConstituentTextArrayPoint>> pointCollection, ref Revision<ConstituentTextArrayPoint> revisionCollection, ref ConstituentTextArrayPoint constituentPoint, INullableReader reader)
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
                            ncs = referenceTimeSeries as TextConstituentArrayTimeSeries;
                            if (ncs != null)
                            {
                                ncs.Add(pointCollection);
                            }
                        }
                    }

                    pointCollection = new SortedList<DateTime, Revision<ConstituentTextArrayPoint>>();
                    revisionCollection = new Revision<ConstituentTextArrayPoint>(valueDate);
                    pointCollection.Add(valueDate, revisionCollection);
                    constituentPoint = new ConstituentTextArrayPoint(declarationDate, valueDate, nonKeyedAttributeSet);
                    revisionCollection.Add(declarationDate, constituentPoint);
                    previousEntityID = entityID;
                    previousTimeSeriesValueID = timeSeriesValueID;
                    previousRelationshipValueID = relationshipValueID;
                    previousValueDate = valueDate;
                    previousDeclarationDate = declarationDate;
                }
                else if (valueDate != previousValueDate)
                {
                    ValidateDueDate(out previousValueDate, out previousDeclarationDate, pointCollection, out revisionCollection, out constituentPoint, valueDate, declarationDate, nonKeyedAttributeSet);
                }
                else if (declarationDate != previousDeclarationDate)
                {
                    constituentPoint = new ConstituentTextArrayPoint(declarationDate, valueDate, nonKeyedAttributeSet);
                    revisionCollection.Add(declarationDate, constituentPoint);
                    previousValueDate = valueDate;
                    previousDeclarationDate = declarationDate;
                }

                if (!requester.EntityLookup.TryGetValue(toEntityID, out entity))
                {
                    if (!args.Translator.TryGetEntityDescriptorByID(toEntityID, codeTypePreference, out entity))
                        entity = new EntityDescriptor(toEntityID);
                }

                constituentPoint.Add(entity, new string[] { value }, nonKeyedAttributeSet);
            }
        }

        private static void ValidateDueDate(out DateTime previousValueDate, out DateTime previousDeclarationDate, SortedList<DateTime, Revision<ConstituentTextArrayPoint>> pointCollection, out Revision<ConstituentTextArrayPoint> revisionCollection, out ConstituentTextArrayPoint constituentPoint, DateTime valueDate, DateTime declarationDate, NonKeyedAttributeSet nonKeyedAttributeSet)
        {
            revisionCollection = new Revision<ConstituentTextArrayPoint>(valueDate);
            pointCollection.Add(valueDate, revisionCollection);
            constituentPoint = new ConstituentTextArrayPoint(declarationDate, valueDate, nonKeyedAttributeSet);
            revisionCollection.Add(declarationDate, constituentPoint);
            previousValueDate = valueDate;
            previousDeclarationDate = declarationDate;
        }

        #endregion
    }
}
