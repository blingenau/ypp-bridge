const child_process = require('child_process');

const child = child_process.spawn('./bin/Release/netcoreapp2.0/win-x86/quartermaster.exe');
child.stdout.on('data', (output) => {
  let data;
  output = JSON.parse(output);
  switch (output.Type) {
    case "PirateInfo":
      data = JSON.parse(output.Data);
      console.log(data); // Output is { pirate: 'Yyuuii', ocean: 'Obsidian' }
      break;
    case "Error":
      console.log('error');
      data = output.Data;
      console.log(data);
      break;  
    default:
      break;
  }
});
child.on('exit', () => {
  console.log('Child process exited');
});

child.stdin.write(JSON.stringify({Action: 'send', Data: 'test'}) + '\n');
child.stdin.write(JSON.stringify({Action: 'getPirateInfo'}) + '\n');
