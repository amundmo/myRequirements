using System;


public interface ISearchableObject
    {
        //enum typess {requirement, securityActivity, threatCategory};
        
        String Name {set; get;}
        String AbstractText { set; get; }
    
    }

