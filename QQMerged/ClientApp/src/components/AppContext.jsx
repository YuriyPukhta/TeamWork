import React from 'react';

export const AppContext = React.createContext(
    {
        auth: false,
        toggleAuth: () => { },
    }
);
