const fs = require('fs');
const content = fs.readFileSync('temp_controllers.txt', 'utf16le');
const lines = content.split('\r\n').map(l => l.trim()).filter(l => l.length > 0 && l !== 'D:\\TD System\\TD Api\\BC.PAYMENT.SYSTEM\\BC.PAYMENT.API\\Controllers\\Prepare\\Account\\AccountReceivablePresetController.cs' && !l.includes('BaseApiController.cs'));
console.log(JSON.stringify(lines, null, 2));
