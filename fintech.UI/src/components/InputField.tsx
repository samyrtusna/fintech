import { Field, ErrorMessage } from "formik";
import type { InputFieldProps } from "../types/componentsTypes";
import { forwardRef, type ForwardedRef } from "react";

const InputField = forwardRef(
  (props: InputFieldProps, ref: ForwardedRef<HTMLInputElement>) => {
    const { id, name, component, type, placeholder } = props;
    return (
      <>
        <Field
          innerRef={ref}
          id={id}
          name={name}
          type={type}
          placeholder={placeholder}
          className="w-full py-1 pl-3 rounded-sm border border-width-thin border-input-border bg-input-bg"
        />
        <div className="min-h-5 pl-3">
          <ErrorMessage
            name={name}
            component={component}
            className="text-xs text-btn-danger"
          />
        </div>
      </>
    );
  },
);

InputField.displayName = "InputField";
export default InputField;
