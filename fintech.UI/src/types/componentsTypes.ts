import type { FormikProps } from "formik";
import type { ComponentPropsWithoutRef } from "react";
import type { IconType } from "react-icons/lib";

export type SubmitButtonProps<T> = { formik: FormikProps<T>; label: string };

export interface InputFieldProps extends ComponentPropsWithoutRef<"input"> {
  id: string;
  name: string;
  component: string;
  type?: string;
  placeholder: string;
}

export type IconProps = { icon: IconType };
