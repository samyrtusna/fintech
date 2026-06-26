import type { SubmitButtonProps } from "../types/componentsTypes";

function SubmitButton<T>(props: SubmitButtonProps<T>) {
  const { formik, label } = props;
  return (
    <div>
      <button
        type="submit"
        disabled={!formik.dirty || !formik.isValid || formik.isSubmitting}
        className="w-full my-2 p-1 rounded-mob-bt-r bg-bt-g text-bt-g-text hover:bg-bt-g-h disabled:bg-gray-400 disabled:cursor-not-allowed cursor-pointer"
      >
        {label}
      </button>
    </div>
  );
}

export default SubmitButton;
