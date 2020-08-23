using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibraryGenFileInfo
{
    public enum E_DBCByteOrder
    {
        E_ByteOrderMotor = 0,
        E_ByteOrderIntel = 1,
    }

    public enum E_DBCValueType
    {
        E_ValueTypeUnsigned = '+',
        E_ValueTypeSigned = '-',
    }
    public class ClassDBCSig
    {
        private string m_name;
        private uint m_startBit;
        private uint m_len;
        private E_DBCByteOrder m_byteOrder;
        private E_DBCValueType m_valueType;
        private double m_factor;
        private double m_offset;
        private double m_minPhyVal;
        private double m_maxPhyVal;
        private string m_unit;
        private string m_receiver;

        public ClassDBCSig(string name, uint startBit, uint len, E_DBCByteOrder byteOrder)
        {
            m_name = name;
            m_startBit = startBit;
            m_len = len;
            m_byteOrder = byteOrder;

        }

        public uint startBit
        {
            get => m_startBit;
            set => m_startBit = value;
        }

        public uint len
        {
            get => m_len;
            set => m_len = value;
        }

        public string name
        {
            get => m_name;
            set => m_name = value;
        }

        public E_DBCByteOrder byteOrder
        {
            get => m_byteOrder;
            set => m_byteOrder = value;
        }

        public E_DBCValueType valueType
        {
            get => (E_DBCValueType)m_valueType;
            set => m_valueType = (E_DBCValueType)value;
        }

        public double factor
        {
            get => m_factor;
            set => m_factor = value;
        }

        public double offset
        {
            get => m_offset;
            set => m_offset = value;
        }

        public double minPhyVal
        {
            get => m_minPhyVal;
            set => m_minPhyVal = value;
        }

        public double maxPhyVal
        {
            get => m_maxPhyVal;
            set => m_maxPhyVal = value;
        }

        public string unit
        {
            get => m_unit;
            set => m_unit = value;
        }

        public string Receiver
        {
            get => m_receiver;
            set => m_receiver = value;
        }
    }
}
