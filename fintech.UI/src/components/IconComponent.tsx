import type { IconProps } from "../types/componentsTypes";

function IconComponent({ icon: Icon }: IconProps) {
  return (
    <div className="flex justify-center items-center w-full h-full p-1 ">
      <Icon className="stroke-1 " />
    </div>
  );
}

export default IconComponent;
