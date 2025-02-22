﻿using System;
using System.Collections.Generic;

namespace LangAnalyzerStd.Morphology
{
    /// <summary>
    /// Морфоатрибут
    /// </summary>
    internal struct MorphoAttributePair
    {
        /// значение
        private readonly MorphoAttributeEnum _morphoAttribute;

        public MorphoAttributePair(MorphoAttributeGroupEnum morphoAttributeGroup, MorphoAttributeEnum morphoAttribute)
        {
            MorphoAttributeGroup = morphoAttributeGroup;
            _morphoAttribute = morphoAttribute;
        }

        /// получение типа атрибута
        public MorphoAttributeGroupEnum MorphoAttributeGroup { get; }

        /// получение значения атрибута
        public MorphoAttributeEnum MorphoAttribute
        {
            get { return _morphoAttribute; }
        }

        public override string ToString()
        {
            return MorphoAttributeGroup + " : " + _morphoAttribute;
        }

        /// получение морфологической информации по форме слова
        /// pWordFormInfo - морфологическая информация о форме слова
        /// morphologyPropertyCount [out] - количество атрибутов
        unsafe public static MorphoAttributeEnum GetMorphoAttribute(BaseMorphoForm baseMorphoForm, MorphoForm morphoForm)
        {
            var result = default(MorphoAttributeEnum);

            var morphoAttributeGroup = baseMorphoForm.MorphoAttributeGroup;
            fixed (MorphoAttributePair* map_ptr = morphoForm.MorphoAttributePairs)
            {
                for (int i = 0, len = morphoForm.MorphoAttributePairs.Length; i < len; i++)
                {
                    var morphoAttributePair = (map_ptr + i);
                    if ((morphoAttributeGroup & morphoAttributePair->MorphoAttributeGroup) == morphoAttributePair->MorphoAttributeGroup)
                    {
                        result |= morphoAttributePair->MorphoAttribute;
                    }
                    else
                    {
                        throw new WrongAttributeException();
                    }
                }
            }
            if (baseMorphoForm.NounType.HasValue)
            {
                var morphoAttributePair = baseMorphoForm.NounType.Value;
                if ((morphoAttributeGroup & morphoAttributePair.MorphoAttributeGroup) == morphoAttributePair.MorphoAttributeGroup)
                {
                    result |= morphoAttributePair.MorphoAttribute;
                }
                else
                {
                    throw new WrongAttributeException();
                }
            }

            return result;
        }
        
        unsafe public static MorphoAttributeEnum GetMorphoAttribute(
            MorphoTypeNative morphoType,
            MorphoFormNative morphoForm,
            ref MorphoAttributePair? nounType)
        {
            var result = default(MorphoAttributeEnum);

            var morphoAttributeGroup = morphoType.MorphoAttributeGroup;
            var len = morphoForm.MorphoAttributePairs.Length;
            if (0 < len)
            {
                fixed (MorphoAttributePair* map_ptr = morphoForm.MorphoAttributePairs)
                {
                    for (len--; 0 <= len; len--)
                    {
                        var morphoAttributePair = (map_ptr + len);
                        if ((morphoAttributeGroup & morphoAttributePair->MorphoAttributeGroup) == morphoAttributePair->MorphoAttributeGroup)
                        {
                            result |= morphoAttributePair->MorphoAttribute;
                        }
                        else
                        {
                            throw new WrongAttributeException();
                        }
                    }
                }
            }
            if (nounType.HasValue)
            {
                var morphoAttributePair = nounType.Value;
                if ((morphoAttributeGroup & morphoAttributePair.MorphoAttributeGroup) == morphoAttributePair.MorphoAttributeGroup)
                {
                    result |= morphoAttributePair.MorphoAttribute;
                }
                else
                {
                    throw new WrongAttributeException();
                }
            }

            return result;
        }

        unsafe public static MorphoAttributeEnum GetMorphoAttribute(
            MorphoTypeNative morphoType,
            MorphoFormNative morphoForm)
        {
            var result = default(MorphoAttributeEnum);

            var len = morphoForm.MorphoAttributePairs.Length;
            if (0 < len)
            {
                var morphoAttributeGroup = morphoType.MorphoAttributeGroup;
                fixed (MorphoAttributePair* map_ptr = morphoForm.MorphoAttributePairs)
                {
                    for (len--; 0 <= len; len--)
                    {
                        var morphoAttributePair = (map_ptr + len);
                        if ((morphoAttributeGroup & morphoAttributePair->MorphoAttributeGroup) == morphoAttributePair->MorphoAttributeGroup)
                        {
                            result |= morphoAttributePair->MorphoAttribute;
                        }
                        else
                        {
                            throw (new WrongAttributeException());
                        }
                    }
                }
            }

            return result;
        }

        unsafe public static MorphoAttributeEnum GetMorphoAttribute(
            MorphoTypeNative morphoType,
            MorphoAttributeEnum morphoAttribute,
            ref MorphoAttributePair? nounType)
        {
            if (nounType.HasValue)
            {
                var morphoAttributeGroup = morphoType.MorphoAttributeGroup;
                var morphoAttributePair = nounType.Value;
                if ((morphoAttributeGroup & morphoAttributePair.MorphoAttributeGroup) == morphoAttributePair.MorphoAttributeGroup)
                {
                    morphoAttribute |= morphoAttributePair.MorphoAttribute;
                }
                else
                {
                    throw new WrongAttributeException();
                }
            }

            return morphoAttribute;
        }
    }
}
