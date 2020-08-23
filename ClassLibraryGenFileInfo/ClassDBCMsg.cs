using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibraryGenFileInfo
{
    public class ClassDBCMsg
    {
        private string m_name;
        private uint m_id;
        private uint m_dlc;
        private E_FrameType m_remoteFlag;
        private E_FrameFormat m_externFlag;

        public ClassDBCMsg(string name, uint id, E_FrameFormat format, E_FrameType type)
        {
            m_name = name;
            m_id = id;
            m_externFlag = format;
            m_remoteFlag = type;
        }

        public string name
        {
            get => m_name;
            set => m_name = value;
        }

        public uint id
        {
            get => m_id;
            set => m_id = value;
        }

        public uint dlc
        {
            get => m_dlc;
            set => m_dlc = value;
        }

        public E_FrameType remoteFlag
        {
            get => default;
            set
            {
            }
        }

        public E_FrameFormat externFlag
        {
            get => default;
            set
            {
            }
        }
    }

    public enum E_FrameFormat
    {
        e_FrameFormat_STD = 0,
        e_FrameFormat_EXT = 1
    }

    public enum E_FrameType
    {
        e_FrameType_DATA = 0,
        e_FrameType_REMOTE = 1
    }
}
