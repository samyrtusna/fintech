import { Field, ErrorMessage } from "formik";
import type { InputFieldProps } from "../types/componentsTypes";

function InputField(props: InputFieldProps) {
  const { id, name, component, type, placeholder } = props;
  return (
    <>
      <Field
        id={id}
        name={name}
        type={type}
        placeholder={placeholder}
        className="w-full py-1 pl-3 rounded-mob-bt-r border border-box-br-c"
      />
      <div className="min-h-5 pl-3">
        <ErrorMessage
          name={name}
          component={component}
          className="text-xs text-red-500"
        />
      </div>
    </>
  );
}

export default InputField;
