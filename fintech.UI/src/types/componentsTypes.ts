import type { FormikProps } from "formik";
import type { LucideIcon } from "lucide-react";

export type SubmitButtonProps<T> = { formik: FormikProps<T>; label: string };

export type InputFieldProps = {
  id: string;
  name: string;
  component: string;
  type?: string;
  placeholder: string;
};

export type IconProps = { icon: LucideIcon };
