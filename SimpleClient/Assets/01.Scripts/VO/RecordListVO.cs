using System;
using System.Collections.Generic;

[Serializable]
public class RecordListVO
{
    public bool result;
    public List<RecordVO> list;
    public int count;

    public RecordListVO(bool result, List<RecordVO> list, int count)
    {
        
    }
}
