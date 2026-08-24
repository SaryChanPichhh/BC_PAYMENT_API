const fs = require('fs');

const fileListContent = fs.readFileSync('temp_controllers.txt', 'utf16le');
const files = fileListContent.split('\r\n').map(l => l.trim()).filter(l => l.length > 0 && l !== 'D:\\TD System\\TD Api\\BC.PAYMENT.SYSTEM\\BC.PAYMENT.API\\Controllers\\Prepare\\Account\\AccountReceivablePresetController.cs' && !l.includes('BaseApiController.cs'));

let successCount = 0;
let errorFiles = [];

for (const file of files) {
    try {
        let content = fs.readFileSync(file, 'utf8');

        // Add using BC.PAYMENT.API.Helper; if not present
        if (!content.includes('using BC.PAYMENT.API.Helper;')) {
            content = 'using BC.PAYMENT.API.Helper;\n' + content;
        }

        // 1. Primary Constructor
        const classRegex = /public\s+class\s+(\w+)\s*:\s*([^{]+)\s*\{/;
        const match = content.match(classRegex);
        if (!match) continue;
        
        const className = match[1];
        const baseClass = match[2].trim();
        const classStartIdx = match.index;
        
        // Find constructor
        const constructorRegex = new RegExp(`public\\s+${className}\\s*\\(([^)]*)\\)\\s*\\{([\\s\\S]*?)\\}`);
        const ctorMatch = content.match(constructorRegex);
        
        if (ctorMatch) {
            const args = ctorMatch[1];
            const body = ctorMatch[2];
            
            // Map parameter names to field names based on body assignments (e.g. _unitOfWork = unitOfWork)
            const paramToField = {};
            const assignments = body.match(/this\.([a-zA-Z_0-9]+)\s*=\s*([a-zA-Z_0-9.]+);|([a-zA-Z_0-9_]+)\s*=\s*([a-zA-Z_0-9.]+);/g);
            if (assignments) {
                for (const assignment of assignments) {
                    const parts = assignment.replace('this.', '').replace(';', '').split('=').map(p => p.trim());
                    if (parts.length === 2) {
                        paramToField[parts[0]] = parts[1];
                    }
                }
            }
            
            // Remove the old fields and constructor
            // We'll replace everything between class { and the end of constructor, or just remove them.
            // Actually, we can just replace the class declaration with primary constructor
            content = content.replace(classRegex, `public class ${className}(${args}) : ${baseClass}\n    {`);
            
            // Remove constructor
            content = content.replace(ctorMatch[0], '');
            
            // Remove private readonly fields that were assigned
            for (const field of Object.keys(paramToField)) {
                const fieldRegex = new RegExp(`\\s*private\\s+readonly\\s+[^;]+?\\s+${field}\\s*;`);
                content = content.replace(fieldRegex, '');
                
                // Replace usage of the field with the mapped value
                const usageRegex = new RegExp(`(?<![a-zA-Z0-9_])(this\\.)?${field}(?![a-zA-Z0-9_])`, 'g');
                content = content.replace(usageRegex, paramToField[field]);
            }
            
            // Remove any empty regions
            content = content.replace(/[ \t]*#region.*?\s*#endregion\s*/g, '');
        }

        // 2. Exception Handler
        content = content.replace(/catch\s*\(\s*SqlException\s+[a-zA-Z0-9_]+\s*\)\s*\{[\s\S]*?\}/g, '');
        content = content.replace(/catch\s*\(\s*Exception\s+([a-zA-Z0-9_]+)\s*\)\s*\{[\s\S]*?\}/g, (m, exName) => {
            // We need to guess the return type T of the method.
            // But it's easier to just use a regex to capture it.
            return `catch (Exception ${exName})\n            {\n                return GlobalExceptionHandler.ExceptionError<REPLACEME>(${exName}.Message);\n            }`;
        });
        
        // 3. Fix REPLACEME with actual generic type from method signature
        // Methods look like: public async Task<ApiResponse<List<DistrictModel>>> GetDistrict()
        const methodRegex = /public\s+(?:async\s+)?(?:Task<)?ApiResponse<([^>]+(?:>[^>]+>|[^>]+))>(?:>)?\s+(\w+)\s*\(/g;
        let methodMatch;
        const methods = [];
        while ((methodMatch = methodRegex.exec(content)) !== null) {
            methods.push({
                type: methodMatch[1],
                index: methodMatch.index
            });
        }
        
        // Split content by methods to replace REPLACEME correctly
        for (let i = 0; i < methods.length; i++) {
            const start = methods[i].index;
            const end = i < methods.length - 1 ? methods[i+1].index : content.length;
            let methodContent = content.substring(start, end);
            
            methodContent = methodContent.replace(/REPLACEME/g, methods[i].type);
            
            // Also replace builder pattern
            // Find `var x = new ApiResponse<T>(); ... return x;`
            // This is hard to regex completely. Let's just do a simpler replacement for the return inside try.
            
            content = content.substring(0, start) + methodContent + content.substring(end);
        }

        // Builder pattern replacement
        // Since the user wants ApiResponse<T>.Builder().WithMessage(...).WithStatusCode(...).WithResult(...).Build();
        // and current code is usually:
        // district.Message = ...; district.Success = ...; district.StatusCode = ...; district.Result = ...; return district;
        // This is too variable.
        
        fs.writeFileSync(file, content, 'utf8');
        successCount++;
    } catch (e) {
        errorFiles.push(file + " - " + e.message);
    }
}

console.log(`Successfully parsed ${successCount} files.`);
console.log("Errors in:", errorFiles);
