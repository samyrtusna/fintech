function Logo({ className }: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <div className="flex justify-center items-center h-full">
      {/* Fixed explicit sizing (e.g., h-16 w-16) or responsive sizing for the icon container */}
      <div className="h-14 w-14 p-1 rounded-full bg-bg-secondary flex items-center justify-center overflow-hidden">
        <img
          src="/Logo.png"
          alt="App Logo"
          className="w-full h-full object-cover"
        />
      </div>
      <h1 className={className}>Finance_it</h1>
    </div>
  );
}

export default Logo;
