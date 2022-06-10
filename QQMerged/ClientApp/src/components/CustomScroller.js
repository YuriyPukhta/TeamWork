import React, { useCallback } from "react";
import { OverlayScrollbarsComponent } from "overlayscrollbars-react";

const CustomScrollbar = React.forwardRef(({ children, ...props }, ref) => {
  const refSetter = useCallback(
    (scrollbarsRef) => {
      if (scrollbarsRef) {
        console.log(scrollbarsRef.osInstance().getElements().viewport);
        ref.current = scrollbarsRef.osInstance().getElements().viewport;
      }
    },
    [ref]
  );

  return (
    <OverlayScrollbarsComponent ref={refSetter} {...props}>
      {children}
    </OverlayScrollbarsComponent>
  );
});

export default CustomScrollbar;
