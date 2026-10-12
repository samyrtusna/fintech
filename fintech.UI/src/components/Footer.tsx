import { AiOutlineLinkedin } from "react-icons/ai";
import {
  FaGithubSquare,
  FaPhoneAlt,
  FaRegCopyright,
  FaWhatsapp,
} from "react-icons/fa";
import { MdLocationPin, MdOutlineEmail } from "react-icons/md";

function Footer() {
  return (
    <div className="w-full mt-5 bg-gray-900 text-gray-200">
      <div className="flex flex-col md:flex-row justify-between px-2 py-5 border-b-thin border-b-gray-500">
        <div className="w-full  md:w-1/2 md:mt-3">
          <h2 className="text-3xl font-bold mb-2 font-serif ">
            Finance<span className="text-yellow-500">_</span>It
          </h2>
          <p className="text-sm">
            A modern app the handle it's own Finances efficiently
          </p>
          <p className="text-sm">
            entrer your daily transactions and let the app do the rest for you!
          </p>
        </div>
        <div className="flex justify-center my-7 md:flex-col md:my-0">
          <a
            href="https://www.linkedin.com/in/samir-mehalli-3bb6472a0/"
            target="_blank"
            rel="noopener noreferrer"
            className="h-10 mx-2 aspect-square p-1 border-thin border-border-subtle md:border-none"
          >
            <AiOutlineLinkedin className="size-full md:size-6" />
          </a>
          <a
            href="https://github.com/samyrtusna/fintech.git"
            target="_blank"
            rel="noopener noreferrer"
            className="h-10 mx-2 aspect-square p-1 border-thin border-border-subtle md:border-none"
          >
            <FaGithubSquare className="size-full md:size-6" />
          </a>
          <a
            href="https://wa.me/213775133811"
            target="_blank"
            rel="noopener noreferrer"
            className="h-10 mx-2 aspect-square p-1 border-thin border-border-subtle md:border-none"
          >
            <FaWhatsapp className="size-full md:size-6" />
          </a>
        </div>
        <div>
          <h2 className="text-xl font-bold mb-2">Contact</h2>
          <div className="flex items-center pb-1">
            <MdLocationPin />
            <h3 className="pl-2 text-sm">55 lot Bekkar, Tizi-Ouzou</h3>
          </div>
          <div className="flex items-center pb-1">
            <MdOutlineEmail />
            <h3 className="pl-2 text-sm">samirTysna@gmail.com</h3>
          </div>
          <div className="flex items-center">
            <FaPhoneAlt className="size-3" />
            <h3 className="pl-2 text-sm">+213 775 133 811</h3>
          </div>
        </div>
      </div>
      <div className="flex justify-center items-center py-3 text-sm">
        <FaRegCopyright className="size-2.5" />
        <h3>2026 Finance_It. All Rights Reserved</h3>
      </div>
    </div>
  );
}

export default Footer;
