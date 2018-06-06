<template>
	<div class="fileLoad">
		<div class="form-group">
			<label for="fileSource">Source:</label>
			<select class="form-control" v-model="fileSource" >
				<option v-for="option in fileSources" v-bind:value="option.value">
					{{ option.label }}
				</option>
			</select>
		</div>
		<div v-if="fileSource==1" class="form-group">
			<label for="fileName">File Name:</label>
			<select class="form-control" v-model="model.fileName" v-on:change="onChangeFile">
				<option value="">Please select one</option>
				<option v-for="option in model.fileList" v-bind:value="option.name">
					{{ option.label }}
				</option>
			</select>
		</div>
		<div v-if="fileSource==2"  class="form-group">
			<label for="file">File Upload:</label>
        	<input  class="form-control" type="file" id="file" ref="file" v-on:change="handleFileUpload()">
		</div>
		
		<div v-if="fileSource==3"  class="form-group">
			<label for="filenm">File Name:</label>
        	<input class="form-control" type="text" id="filenm"  v-model="model.fileName">
		</div>
		<div v-if="fileSource!=1"  class="form-group">
			<label for="filen">File Label:</label>
        	<input class="form-control" type="text" id="filen"  v-model="model.fileLabel">
		</div>
		<div  class="form-group">
			<label for="decodeKey">Decode Key:</label>
			<input class="form-control" id="decodeKey" type="password"   v-model="model.fileKey" />
		</div>
		<div v-if="fileSource==2"  class="form-group">
			<input class="form-control" type="button" value="Upload" @click="uploadFile()">
		</div>
		<div v-if="fileSource==1"  class="form-group">
			<input class="form-control" type="button" value="Load" @click="loadFile()">
		</div>
		<div v-if="fileSource==1"  class="form-group">
			<input class="form-control" type="button" value="Delete" @click="deleteFile()">
		</div>
		<div v-if="fileSource==3"  class="form-group">
			<input class="form-control" type="button" value="Create New" @click="createFile()">
		</div>
	</div>
</template>
<script>
	import fileLoaderController from '@/components/fileLoader/fileLoaderController'
	import {EventBus} from '@/services/busService.js'
	import Vue from 'vue'
	
	export default {
		name:'fileLoader',
		data() {
			return  {
				model:{
					fileName: '',
					fileKey: '',
					fileLabel:'',
					fileList:[]
				},
				file:'',
				shouldLoadFile:true,
				fileSources:[
					{label:'Existing',value:1},
					{label:'Upload',value:2},
					{label:'New',value:3}],
				fileSource:1
			}
		},
		mounted(){
			this.loadAvailableFiles();
		},
		methods:{
			createFile:function(){
				var context = this;
				fileLoaderController.createFile({
						FileName:context.model.fileName,
						FileLabel:context.model.fileLabel,
						FileKey:context.model.fileKey,
					},function(data){
						context.fileSource=1;
						context.loadAvailableFiles();});
			},
			onChangeFile:function(){
				this.shouldLoadFile= 
					this.model.fileName==undefined || 
					this.model.fileName==null ||
					this.model.fileName=='';
			},
			uploadFile(){
				var context = this;
				fileLoaderController.uploadFile(
					context.file,
					context.model.fileLabel,
					context.model.fileKey,function(data){
						context.fileSource=1;
						context.loadAvailableFiles();});
			},
			deleteFile(){
				var context = this;
				if(true==confirm("Delete file "+context.model.fileName)){
					fileLoaderController.deleteFile(context.model.fileName,
						function(data){
							context.loadAvailableFiles();
						});
				}
			},
			handleFileUpload(){
				this.file = this.$refs.file.files[0];
			},
			loadAvailableFiles:function(){
				var context = this;
				fileLoaderController.loadFileList(function(result){
					while(context.model.fileList.length>0){
						context.model.fileList.pop();
					}
					console.log(result);
					for(var i=0;i<result.length;i++){
						context.model.fileList.push({
							name:result[i].FileName,
							label:result[i].FileLabel
						});
					};
					//Vue.$set(context.model, 'fileList', fileList);
				});
			},
			loadFile:function(){
				var context = this;
				fileLoaderController.loadFile({
					FileName:context.model.fileName,
					FileKey:context.model.fileKey
				},function(data){
					EventBus.$emit('authEvent',{
						event:'fileLoaded'
					});
					context.$router.push({ name: 'tree'});
				});
			}
			
		}
	}
</script>
<style>
  .dropbox {
    outline: 3px dashed grey; /* the dash box */
    outline-offset: -10px;
    background: lightcyan;
    color: dimgray;
    padding: 5px 5px;
    min-height: 30px; /* minimum height */
    position: relative;
    cursor: pointer;
  }

  .input-file {
    opacity: 0; /* invisible but it's there! */
    width: 100%;
    height: 30px;
    position: absolute;
    cursor: pointer;
  }

  .dropbox:hover {
    background: lightblue; /* when mouse over to the drop zone, change color */
  }

  .dropbox p {
    font-size: 1.2em;
    text-align: center;
    padding: 50px 0;
  }
</style>